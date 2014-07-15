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
--     地图：地图和年级科目对应，每一个年级的每一学期的每一科目，应该都有一个地图
--     关卡：每个单元对应一个关卡
--     题目：题目是趣味学堂的核心，题目的具体信息，应该到题库去获取，但是此处需要一张，关卡和题目的对应表
--
--     藏宝图碎片：藏宝图是和前面的关卡地图是不同的概念，藏宝图是用来寻找宝物的，
--                 需要一张记录学生何时获得了何种藏宝图碎片的记录
--     宝物：各种宝物的数据是作为配置存在的，需要一张表记录，重点需要一张表记录学生何时获得了何种宝物
--     宝屋：学生打通一张地图后，可以打开神秘宝屋，它没有数据，只是给学生获得神秘高级宝物机会的一个形象表示，
--           对开发者不是一个实在的对象
--     奖状：学生经验值增加提升等级后，或者打通关卡后，可以领取奖状，需要记录奖状信息，
--           学生获得记录，学生申请邮寄记录
--
--     乐币：虚拟币，余额记录在学生虚拟币表里，而不放在学生信息表里，因为两者的安全级别不同
--     银币：虚拟币，余额记录在学生虚拟币表里，另需要获得和消费情况
--

-- if not exist database app_intesch;
-- create database app_intesch;
use app_intesch;

------------------------------------------------------------------------------------
-- 与学生相关的表
-- 表1：学生信息 动态表
create table students(
	ID int,
	signin int,      -- 已连续签到天数
	exp_value int,   -- EXP|empirical value|empiric value 经验值，私有
	prop_num  int,   -- 道具数量，私有
	certi_num int,   -- 奖状数，公开
	lower_gem  int,  -- 低级宝物数量，公开，宝物是分级的，这里分三级宝物，
	                 -- 可以用独立宝物表来存储宝物数量，更易于扩展，但会增加计算量
	middle_gem int,  -- 中极宝物，公开
	high_gem   int,  -- 高极宝物，公开
	primary key(ID)
);

create index exp_index on students(exp_value);
create index gem_index on students(high_gem, middle_gem, lower_gem);
create index certi_index on students(certi_num);

-- 表2：签到 动态表
create table student_signs (
	ID int,
	student_id int,  -- 学生ID
	sign_date int,   -- 签到日期，这里int型表示
	signin bit,      -- 是否签到，学生进入趣味学堂，需要手动签到
	primary key(ID),
);

create index sign_index on student_signs(student_id);

-- 表3：登录 动态表
--      此表与趣味学堂的功能并无影响，但是对分析用户活跃度，出错时排错等很有用
create table student_logins (
	ID int,
	student_id int,  -- 学生ID
	loadin int,      -- 进入时间
	loadout int,     -- 退出时间
	ipaddress varchar(255),
	source_type varchar(255), -- 记录来源，客户端、移动端、浏览器，尽量详细，
	                          -- 客户端尽量带版本号，浏览器尽量带版本和版本号
	primary key(ID),
);

create index login_index on student_logins(student_id);

-- 任务   这个在服务器可以不存在，客户端自己制作
create table tasks(
	ID int,
	serial int,     -- 任务的序列号
	content varchar(2560), -- 指示的某个操作，
	primary key(ID)
);

--------------------------------------------------------------------------------
-- 与地图相关的表
-- 表3：地图 这是趣味学堂的配置表， 系统初始表
create table maps (
	ID int,
	name varchar(255),
	descep varchar(1024),
	_subject varchar(255), -- 对应科目
	semester int,         -- 哪个学期，也就是哪一册
	edition int,          -- 版本
	checkpoints int,      -- 关卡总数
	primary key(ID)
);

-- 表4：地图内容  系统初始表
create table map_contents (
	ID int,
	map_id int,
	cp_id  int,
	serial int, -- 序号，各关卡在地图中的排序
	primary key(ID)
);

create index map_checkpoint_index on map_contents (map_id);

-- 表5：学生探索地图记录 动态表
create table student_maps (
	ID int,
	student_id int,  -- 学生ID
	map_id int,
	checkpoints int, -- 已经通过的关卡数
	last_complete datetime, -- 最后通关时间
	primary key(ID)
);

create index student_index on student_maps(student_id);
create index map_index on student_maps(map_id);

-- 表6：关卡 系统初始表
create table checkpoints(
	ID int,
	name varchar(255),
	remark varchar(255),
	unit int,  -- 对应教材的单元数
	primary key(ID)
);

-- 表7：学生关卡  动态表
create table student_checkpoints(
	ID int,
	student_id int,  -- 学生ID
	map_id int,      -- 数据冗余，实际上根据cp_id，即可找到map_id，
	                 -- 因为一个关卡应该只出现在一个地图中
	cp_id int,       -- 关卡ID
	ratio int,
	primary key(ID)
);

create index student_checkpoint_index on student_checkpoints(student_id, map_id);

-- 表8：关卡内容  系统初始表
create table cp_contents(
	ID int,
	checkpoint_id int,  -- 关卡ID
	content_id int,     -- 题库中题目的ID，由这个ID，应该可以从题库中提取出题目内容
	serial int,         -- 题目在关卡中排序
	primary key(ID)
);

create index checkpoint_index on cp_contents(checkpoint_id, serial);

-- 表9：学生题目  动态表
create table student_contents(
	ID int,
	student_id int,    -- 学生ID
	checkpoint_id int, -- 关卡ID
	content_id int,    -- 此处是cp_contents表的ID，用它可以查到题库中题目的ID
	answer varchar(255),  -- 问题答案
	score int, 
	comp_time datetime, 
	primary key(ID)
);

------------------------------------------------------------------------------------
-- 物品体系
-- 表10：道具 配置表
create table props(
	ID int,
	name varchar(255),
	photo varchar(255),
	remark varchar(255),
	prices int, 
	primary key(ID)
);

-- 表11：道具账单  获取和使用道具的记录 购买记录在银币账单里同时出现 动态表
create table prop_billes(
	ID int,
	student_id int,
	prop_id int,
	number int,  -- 负数表示使用掉，正数表示获取，可能是系统赠送，也可能是购买
	remark varchar(255),
	primary key(ID)
);

create index use_prop_index on prop_billes(student_id);

-- 表12：宝物 配置表
create table gems(
	ID int,
	name   varchar(255),
	photo  varchar(255),
	remark varchar(255),
	gem_rank int, 
	primary key(ID)
);

-- 表13：领取宝物记录 动态表
create table gem_records(
	ID int,
	student_id int,
	gem_id int,
	reason varchar(255), -- 获得原因，描述一下学生是怎么获得的，完成哪些任务
	get_time datetime,
	primary key(ID)
);

create index get_gem_index on gem_records(student_id);

-- 表14：奖状 配置表
create table certis(
	ID int,
	name varchar(255),
	photo varchar(255),
	remark varchar(255),
	primary key(ID)
);

-- 表15：学生获取奖状记录 动态表
create table certi_gets(
	ID int,
	student_id int,
	certi_id int,
	reason  varchar(255), -- 获得原因，描述一下学生是怎么获得的，等级提升哪级，或者完成了什么任务
	get_time datetime,
	primary key(ID)
);

create index get_certi_index on certi_gets(student_id);

-- 表16：邮寄奖状记录  动态表
create table certi_mails(
	ID int,
	student_id int,
	certi_id int,
	reason  varchar(255), -- 获得原因，描述一下学生是怎么获得的，等级提升哪级，或者完成了什么任务
    mail_time datetime,
	primary key(ID)
);

create index mail_certi_index on certi_mails(student_id);

---------------------------------------------------------------------------------------
-- 表17：虚拟币余额  动态表
create table student_coins(
	ID int,
	student_id int,    -- 学生ID
	iscoin int default 0, 
	svcoin int default 0, 
	primary key(ID)
);

-- 表18：乐币账单  动态表
create table iscoin_billes(
	ID int,
	student_id int, -- 学生ID
	iscoin int,     -- 正数表示收入，负数表示输出
	good_id int,    -- 购买的商品ID，
	good_type varchar(255), -- 商品类型
	remark varchar(2560), 
	op_time datetime,
	primary key(ID)
);

-- 表19：银币账单  动态表
create table silber_billes(
	ID int,
	student_id int, -- 学生ID
	svcoin int,     -- 正数表示收入，负数表示输出
	good_id int,    -- 购买的商品ID，
	good_type varchar(255), -- 商品类型
	remark varchar(2560), 
	op_time datetime,
	primary key(ID)
);

-- 表20：充值记录  动态表
create table rechanges(
	ID int,
	student_id int,    -- 学生ID
	iscoin int, 
	op_time datetime,
	primary key(ID)
);

-- 表21：兑换记录  动态表
create table exchanges(
	ID int,
	student_id int,    -- 学生ID
	silvercoin int, 
	op_time datetime,
	primary key(ID)
);

