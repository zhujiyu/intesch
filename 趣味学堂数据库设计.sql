-- 趣味学堂数据库
-- 乐教乐学 研发部 朱继玉
-- 2014.07.14

-- 趣味学堂中，共有如下对象：
--
--     学生：学生是一切活动的操作主体，云平台上已经有了学生表，但此处需要一张新的学生信息表，
--           记录学生在趣味学堂的状态，和获得的地图、宝物、奖状等的数量
--     任务：任务是引导学生如何使用趣味学堂的一组数据，充当使用教程的任务
--           这个可以不体现在数据库，客户端自己去实现就行了
--
--     地图：地图和年级科目对应，每一个年级的每一学期的每一科目对应一张地图
--     关卡：每个单元对应一个关卡
--     题目：题目是趣味学堂的核心，题目的具体信息，应该到题库去获取，
--           但是此处需要一张，关卡和题目的对应表
--
--     藏宝图碎片：藏宝图是和前面的关卡地图是不同的概念，藏宝图是用来寻找宝物的，
--                 需要一张记录学生何时获得了何种藏宝图碎片的记录
--     宝物：各种宝物的数据是作为配置存在的，需要一张表记录，
--           重点需要一张表记录学生何时获得了何种宝物
--     宝屋：学生打通一张地图后，可以打开神秘宝屋，它没有数据，
--           只是给学生获得神秘高级宝物机会的一个形象表示，
--           对开发者不是一个实在的对象
--     奖状：学生经验值增加提升等级后，或者打通关卡后，可以领取奖状，需要记录奖状信息，
--           学生获得记录，学生申请邮寄记录
--
--     乐币：虚拟币，余额记录在学生虚拟币表里，而不放在学生信息表里，
--           因为两者的安全级别不同，另外需要记录充值和消费情况的账单
--     银币：虚拟币，余额记录在学生虚拟币表里，另需要记录获得和消费情况的账单
--

-- if not exist database app_intesch;
-- create database app_intesch;
use app_intesch;

---------------------------------------------------------------------------------
-- 与学生相关的表
-- 表1：学生信息 动态表
if exists(select * from sysobjects where name = 'students')
drop table students;

create table students(
	ID int,          -- 学生编号
	signin int,      -- 已连续签到天数
	exp_value int,   -- EXP|empirical value|empiric value 经验值，私有
	max_constant_rights int default 0, -- 历史最大连对数
	constant_rights int default 0,     -- 当前连对数，这个是各科目的合计
	total_rights int default 0,        -- 累计做对题目数
--	prop_num  int default 0,   -- 道具数量，私有
	treasure_frag int default 0, -- 藏宝图碎片数
	certi_num  int default 0,  -- 奖状数，公开
	lower_gem  int default 0,  -- 低级宝物数量，公开，宝物是分级的，这里分三级宝物，
	                 -- 可以用独立宝物表来存储宝物数量，更易于扩展，但会增加计算量
	middle_gem int default 0,  -- 中极宝物，公开
	high_gem   int default 0,  -- 高极宝物，公开
	primary key(ID)
)

create index exp_index on students(exp_value);
create index gem_index on students(high_gem, middle_gem, lower_gem);
create index certi_index on students(certi_num);

-- 表2：签到 动态表
if exists(select * from sysobjects where name = 'student_signs')
drop table student_signs;

create table student_signs (
	ID int identity(1000000, 1),
	student_id int,  -- 学生ID
	sign_date int,   -- 签到日期，这里int型表示
	signin bit,      -- 是否签到，学生进入趣味学堂，需要手动签到
	primary key(ID),
);

create index sign_index on student_signs(student_id);

-- 表3：登录 动态表
--      此表用于判定学生当前是否在线，另外对分析用户活跃度，出错时排错等很有用
if exists(select * from sysobjects where name = 'student_logins')
drop table student_logins;

create table student_logins (
	ID int identity(1000000, 1),
	student_id int,  -- 学生ID
	loadin int,      -- 进入时间
	loadout int,     -- 退出时间，利用该字段，可以粗略估计用户在线状态
	ipaddress   varchar(255),
	source_type varchar(255), -- 记录来源，客户端、移动端、浏览器，尽量详细，
	                          -- 客户端尽量带版本号，浏览器尽量带版本和版本号
	primary key(ID),
);

create index login_index on student_logins(student_id);

--------------------------------------------------------------------------------
-- 与地图相关的表
-- 表4：地图 这是趣味学堂的配置表， 系统初始表
if exists(select * from sysobjects where name = 'maps')
drop table maps;

create table maps (
	ID int identity(1000000, 1),
	name     varchar(255),
	remark   varchar(1024),
	_subject varchar(255), -- 对应科目
	semester int,         -- 哪个学期，也就是哪一册
	edition int,          -- 版本
	checkpoints int,      -- 关卡总数
	primary key(ID)
);

---- 表5：地图内容  系统初始表
--create table map_contents (
--	ID int,
--	map_id int,
--	cp_id  int,
--	serial int, -- 序号，各关卡在地图中的排序
--	primary key(ID)
--);

--create index map_checkpoint_index on map_contents (map_id);

-- 表5：学生探索地图记录 动态表
if exists(select * from sysobjects where name = 'student_maps')
drop table student_maps;

create table student_maps (
	ID int identity(1000000, 1),
	student_id int,  -- 学生ID
	map_id int,
	max_constant_rights int default 0, 
	constant_rights int default 0, 
	total_rights int default 0,
	checkpoints int default 0, -- 已经通过的关卡数
	reward bit default 0,      -- 宝屋是否打开，各关卡通关后，可以打开宝屋，状态不可逆
	last_complete datetime,    -- 最后通关时间
	primary key(ID)
);

create index map_student_index on student_maps(student_id, map_id); -- 获取一个学生在某个地图的信息
create index map_constant_right_index on student_maps (map_id, max_constant_rights); -- 最高连对排行
create index map_total_right_index on student_maps (map_id, total_rights);           -- 累计做对题目排行
--create index map_index on student_maps(map_id);
--create index map_complete_index on student_maps (last_complete);

-- 表6：关卡 系统初始表
--      一个关卡应该只在一个地图中，所在地图可以算成关卡的一个属性
--      所以无需专门的地图和关卡的对应表，
if exists(select * from sysobjects where name = 'checkpoints')
drop table checkpoints;

create table checkpoints(
	ID int identity(1000000, 1),
	map_id int, -- 所在地图
	serial int, -- 序号，各关卡在地图中的排序
	unit   int, -- 对应教材的单元数
	name   varchar(255),
	remark varchar(255),
	primary key(ID)
);

create index checkpoint_map_index on checkpoints (map_id, serial);

-- 表7：学生关卡  动态表
if exists(select * from sysobjects where name = 'student_checkpoints')
drop table student_checkpoints;

create table student_checkpoints(
	ID int identity(1000000, 1),
	student_id int,  -- 学生ID
	map_id int,      -- 数据冗余，实际上根据cp_id，即可找到map_id，
	                 -- 因为一个关卡应该只出现在一个地图中
	cp_id int,       -- 关卡ID
	right_num int,   -- 答对的题目数
	score int,       -- 一个关卡实际上相当于一份试卷，根据各题目的分值计算总分
	pass bit default 0,      -- 是否通关，通关是不可逆过程，防止学生作弊，奖励不会重复发放
	                         -- 即学生通过一关后，即使把已做的题目重新做错，
	                         -- 答对题目数和得分会相应修改，但通关状态不会变
	                         -- 通关后即发放奖励，包括银币和宝图碎片
--	silver bit default 0,    -- 银币奖励是否发给学生
--	treasure bit default 0,  -- 宝图碎片是否发给学生
--	                         -- 奖励和宝图碎片每个单元只能发一次，为防止学生作弊，必须记录结果
--	                         -- 否则，学生可以通过新把题目都做对，然后再做错几道，
--	                         -- 然后再重新做对，再次触发关卡通关事件
	primary key(ID)
);

create index student_map_index on student_checkpoints(student_id, map_id);
create index student_checkpoint_index on student_checkpoints(student_id, cp_id);

-- 表8：关卡内容  系统初始表
if exists(select * from sysobjects where name = 'cp_contents')
drop table cp_contents;

create table cp_contents(
	ID int identity(1000000, 1),
	checkpoint_id int,  -- 关卡ID
	serial int,         -- 题目在关卡中排序
	problem_id int,     -- 题库中题目的ID，由这个ID，应该可以从题库中提取出题目内容
	exp_value int,
	silver int,
	primary key(ID)
);

create index checkpoint_content_index on cp_contents (problem_id);
create index checkpoint_serial_index on cp_contents (checkpoint_id, serial);

-- 表9：习题回答  动态表
if exists(select * from sysobjects where name = 'content_replies')
drop table content_replies;

create table content_replies(
	ID int identity(1000000, 1),
	student_id int, -- 学生ID
	map_id int,     -- 地图ID
	checkpoint_id int, -- 关卡ID
	problem_id int,    -- 题库中题目的ID，由这个ID，应该可以从题库中提取出题目内容
	answer varchar(255), -- 问题答案
	judge bit default 0, -- 对错判定
	score int, 
	exp_value int,     -- 该字段记录学生本次做对题目获得的经验值，
	                   -- 一条新记录产生时，该值等于cp_contents表中exp_value字段的值
	                   -- 之后每次答案修改为正确值，该字段的值会减小一半
	silver int,        -- 使用方法同exp_value字段
	comp_time datetime, 
	primary key(ID)
);

create index reply_index on content_replies (student_id, map_id, checkpoint_id, problem_id);
--create index reply_judge_index on content_replies (student_id, judge, comp_time);

-- 表10：连对log  动态表
if exists(select * from sysobjects where name = 'constant_right_log')
drop table constant_right_log;

create table constant_right_log(
	ID int identity(1000000, 1),
	student_id int, -- 学生ID
	map_id int,     -- 地图ID
	constant_rights int, 
	log_time datetime, 
	primary key(ID)
);

create index constant_right_index on constant_right_log (student_id, map_id);

-- 表11：连对log  动态表
if exists(select * from sysobjects where name = 'card_log')
drop table card_log;

create table card_log(
	ID int identity(1000000, 1),
	student_id int, -- 学生ID
	map_id int,     -- 地图ID
	card_id int,    -- 使用道具卡的编号
	log_time datetime, 
	primary key(ID)
);

create index card_log_index on card_log (student_id, map_id, card_id);

----------------------------------------------------------------------------------
-- 物品体系
-- 表12：宝物 配置表
if exists(select * from sysobjects where name = 'gems')
drop table gems;

create table gems(
	ID int identity(1000000, 1),
	name   varchar(255),
	photo  varchar(255),
	remark varchar(255),
	gem_rank int, 
	primary key(ID)
);

create index gem_name_index on gems(name);

-- 表13：领取宝物记录 动态表
if exists(select * from sysobjects where name = 'gem_rewards')
drop table gem_rewards;

create table gem_rewards(
	ID int identity(1000000, 1),
	student_id int,
	gem_id int,
--	reason varchar(255), -- 获得原因，描述一下学生是怎么获得的，完成哪些任务
	get_time datetime,
	primary key(ID)
);

create index reward_gem_index on gem_rewards(student_id);

-- 表14：奖状 配置表
if exists(select * from sysobjects where name = 'certis')
drop table certis;

create table certis(
	ID int identity(1000000, 1),
	name   varchar(255),
	photo  varchar(255),
	remark varchar(255),
	primary key(ID)
);

create index certi_name_index on certis(name);

-- 表15：学生获取奖状记录 动态表
if exists(select * from sysobjects where name = 'certi_rewards')
drop table certi_rewards;

create table certi_rewards(
	ID int identity(1000000, 1),
	student_id int,
	certi_id int,
	reason  varchar(255),  -- 获得原因，描述一下学生是怎么获得的，
	                       -- 等级提升哪级，或者完成了什么任务
	applied bit default 0, -- 是否申请邮寄
	get_time     datetime default 0, -- 获得奖励的时间，这个时间该条记录一产生就有值
    mail_time    datetime default 0, -- 发出奖状的时间，
                                     -- 公司服务人员发出奖状后，这个字段才有值
    receive_time datetime default 0, -- 确认收到的时间，学生确认收到，这个字段才有值
	primary key(ID)
);

create index reward_certi_index on certi_rewards(student_id, certi_id);

---- 表14：邮寄奖状记录  动态表
--if exists(select * from sysobjects where name = 'certi_mails')
--drop table certi_mails;

--create table certi_mails(
--	ID int,
--	student_id int,
--	reward_id  int,   -- certi_reward表的ID
	
--    mail_time  datetime,
--	primary key(ID)
--);

--create index mail_certi_index on certi_mails(student_id, reward_id);

-- 表16：道具 配置表
if exists(select * from sysobjects where name = 'props')
drop table props;

create table props(
	ID int identity(1000000, 1),
	name   varchar(255),
	photo  varchar(255),
	remark varchar(255),
	prices int, 
	primary key(ID)
);

-- 表17：道具账单  获取和使用道具的记录 购买记录在银币账单里同时出现 动态表
if exists(select * from sysobjects where name = 'prop_billes')
drop table prop_billes;

create table prop_billes(
	ID int identity(1000000, 1),
	student_id int,
	prop_id int,
	number int,  -- 负数表示使用掉，正数表示获取，可能是系统赠送，也可能是购买
	remark varchar(255),
	primary key(ID)
);

create index use_prop_index on prop_billes(student_id);

---------------------------------------------------------------------------------------
-- 表18：虚拟币余额  动态表
if exists(select * from sysobjects where name = 'virtual_coins')
drop table virtual_coins;

create table virtual_coins(
	ID int identity(1000000, 1),
	student_id int,    -- 学生ID
	iscoin int default 0, 
	svcoin int default 0, 
	primary key(ID)
);

create index virtual_coin_index on virtual_coins (student_id);

-- 表19：乐币账单  动态表
if exists(select * from sysobjects where name = 'iscoin_billes')
drop table iscoin_billes;

create table iscoin_billes(
	ID int identity(1000000, 1),
	student_id int, -- 学生ID
	iscoin int,     -- 正数表示收入，负数表示输出
	good_id int,    -- 购买的商品ID，
	good_type varchar(255), -- 商品类型
	remark varchar(2560), 
	op_time datetime,
	primary key(ID)
);

create index iscoin_bill_index on iscoin_billes (student_id);

-- 表20：银币账单  动态表
if exists(select * from sysobjects where name = 'silver_billes')
drop table silver_billes;

create table silver_billes(
	ID int identity(1000000, 1),
	student_id int, -- 学生ID
	svcoin int,     -- 正数表示收入，负数表示输出
	good_id int,    -- 购买的商品ID，
	good_type varchar(255), -- 商品类型
	remark varchar(2560), 
	op_time datetime,
	primary key(ID)
);

create index silver_bill_index on silver_billes (student_id);

---- 表20：充值记录  动态表
--create table rechanges(
--	ID int,
--	student_id int,    -- 学生ID
--	iscoin int, 
--	op_time datetime,
--	primary key(ID)
--);

---- 表21：兑换记录  动态表
--create table exchanges(
--	ID int,
--	student_id int,    -- 学生ID
--	silvercoin int, 
--	op_time datetime,
--	primary key(ID)
--);

