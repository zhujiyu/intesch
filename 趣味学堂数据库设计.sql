-- Ȥζѧ�����ݿ�
-- �ֽ���ѧ �з��� �����
-- 2014.07.14

-- Ȥζѧ���У��������¶���
--
--     ѧ����ѧ����һ�л�Ĳ������壬��ƽ̨���Ѿ�����ѧ�������˴���Ҫһ���µ�ѧ����Ϣ��
--           ��¼ѧ����Ȥζѧ�õ�״̬���ͻ�õĵ�ͼ�������״�ȵ�����
--     ��������������ѧ�����ʹ��Ȥζѧ�õ�һ�����ݣ��䵱ʹ�ý̵̳�����
--           ������Բ����������ݿ⣬�ͻ����Լ�ȥʵ�־�����
--
--     ��ͼ����ͼ���꼶��Ŀ��Ӧ��ÿһ���꼶��ÿһѧ�ڵ�ÿһ��Ŀ��Ӧ�ö���һ����ͼ
--     �ؿ���ÿ����Ԫ��Ӧһ���ؿ�
--     ��Ŀ����Ŀ��Ȥζѧ�õĺ��ģ���Ŀ�ľ�����Ϣ��Ӧ�õ����ȥ��ȡ�����Ǵ˴���Ҫһ�ţ��ؿ�����Ŀ�Ķ�Ӧ��
--
--     �ر�ͼ��Ƭ���ر�ͼ�Ǻ�ǰ��Ĺؿ���ͼ�ǲ�ͬ�ĸ���ر�ͼ������Ѱ�ұ���ģ�
--                 ��Ҫһ�ż�¼ѧ����ʱ����˺��ֲر�ͼ��Ƭ�ļ�¼
--     ������ֱ������������Ϊ���ô��ڵģ���Ҫһ�ű��¼���ص���Ҫһ�ű��¼ѧ����ʱ����˺��ֱ���
--     ���ݣ�ѧ����ͨһ�ŵ�ͼ�󣬿��Դ����ر��ݣ���û�����ݣ�ֻ�Ǹ�ѧ��������ظ߼���������һ�������ʾ��
--           �Կ����߲���һ��ʵ�ڵĶ���
--     ��״��ѧ������ֵ���������ȼ��󣬻��ߴ�ͨ�ؿ��󣬿�����ȡ��״����Ҫ��¼��״��Ϣ��
--           ѧ����ü�¼��ѧ�������ʼļ�¼
--
--     �ֱң�����ң�����¼��ѧ������ұ����������ѧ����Ϣ�����Ϊ���ߵİ�ȫ����ͬ
--     ���ң�����ң�����¼��ѧ������ұ������Ҫ��ú��������
--

-- if not exist database app_intesch;
create database app_intesch;

----------------------------------------------------------------------------------------------------------------
-- ��ѧ����صı�
-- ��1��ѧ����Ϣ ��̬��
create table students(
	ID int,
	signin int,      -- ������ǩ������
	exp_value int,   -- EXP|empirical value|empiric value ����ֵ��˽��
	prop_num  int,   -- ����������˽��
	certi_num int,   -- ��״��������
	lower_gem  int,  -- �ͼ����������������������Ƿּ��ģ������������������ö�����������洢������������������չ���������Ӽ�����
	middle_gem int,  -- �м��������
	high_gem   int,  -- �߼��������
	primary key(ID),
--	index key (exp_value),
--	index (high_gem, middle_gem, lower_gem),
--	index (certi_num)
);

create index exp_index on students(exp_value);
create index gem_index on students(high_gem, middle_gem, lower_gem);
create index certi_index on students(certi_num);

-- ��2����¼ǩ�� ��̬��
create table student_sign (
	ID int,
	student_id int,  -- ѧ��ID
	signin bit,      -- �Ƿ�ǩ����ѧ������Ȥζѧ�ã���Ҫ�ֶ�ǩ��
	loadin int,      -- ����ʱ��
	loadout int,     -- �˳�ʱ��
	ipaddress varchar(255),
	source_type varchar(255), -- ��¼��Դ���ͻ��ˡ��ƶ��ˡ��������������ϸ���ͻ��˾������汾�ţ�������������汾�Ͱ汾��
	primary key(ID),
);

-- ����   ����ڷ��������Բ����ڣ��ͻ����Լ�����
create table tasks(
	ID int,
	serial int,     -- ��������к�
	content varchar(2560), -- ָʾ��ĳ��������
	primary key(ID)
);

----------------------------------------------------------------------------------------------------------------
-- ���ͼ��صı�
-- ��3����ͼ ����Ȥζѧ�õ����ñ� ϵͳ��ʼ��
create table maps (
	ID int,
	name varchar(255),
	descep varchar(1024),
	_subject varchar(255), -- ��Ӧ��Ŀ
	semester int,         -- �ĸ�ѧ�ڣ�Ҳ������һ��
	edition int,          -- �汾
	checkpoints int,      -- �ؿ�����
	primary key(ID)
);

-- ��4����ͼ����  ϵͳ��ʼ��
create table map_contents (
	ID int,
	map_id int,
	cp_id  int,
	serial int, -- ��ţ����ؿ��ڵ�ͼ�е�����
	primary key(ID)
);

create index map_checkpoint_index on map_contents (cp_id);

-- ��5��ѧ��̽����ͼ��¼ ��̬��
create table student_maps (
	ID int,
	student_id int,  -- ѧ��ID
	map_id int,
	checkpoints int, -- �Ѿ�ͨ���Ĺؿ���
	last_complete datetime, -- ���ͨ��ʱ��
	primary key(ID)
);

create index student_index on student_maps(student_id);
create index map_index on student_maps(map_id);

-- ��6���ؿ� ϵͳ��ʼ��
create table checkpoints(
	ID int,
	name varchar(255),
	remark varchar(255),
	unit int,  -- ��Ӧ�̲ĵĵ�Ԫ��
	primary key(ID)
);

-- ��7��ѧ���ؿ�  ��̬��
create table student_checkpoints(
	ID int,
	student_id int,  -- ѧ��ID
	map_id int,      -- �������࣬ʵ���ϸ���cp_id�������ҵ�map_id����Ϊһ���ؿ�Ӧ��ֻ������һ����ͼ��
	cp_id int,       -- �ؿ�ID
	ratio int,
	primary key(ID)
);

create index student_checkpoint_index on student_checkpoints(student_id, cp_id);

-- ��8���ؿ�����  ϵͳ��ʼ��
create table cp_contents(
	ID int,
	checkpoint_id int,  -- �ؿ�ID
	content_id int,     -- �������Ŀ��ID�������ID��Ӧ�ÿ��Դ��������ȡ����Ŀ����
	serial int,         -- ��Ŀ�ڹؿ�������
	primary key(ID)
);

create index checkpoint_index on cp_contents(checkpoint_id, serial);

-- ��9��ѧ����Ŀ  ��̬��
create table student_contents(
	ID int,
	student_id int,    -- ѧ��ID
	checkpoint_id int, -- �ؿ�ID
	content_id int,    -- �˴���cp_contents���ID���������Բ鵽�������Ŀ��ID
	answer varchar(255),  -- �����
	score int, 
	comp_time datetime, 
	primary key(ID)
);

--------------------------------------------------------------------------------------------------------
-- ��Ʒ��ϵ
-- ��10������ ���ñ�
create table props(
	ID int,
	name varchar(255),
	photo varchar(255),
	remark varchar(255),
	prices int, 
	primary key(ID)
);

-- ��11�������˵�  ��ȡ��ʹ�õ��ߵļ�¼ �����¼�������˵���ͬʱ���� ��̬��
create table prop_billes(
	ID int,
	student_id int,
	prop_id int,
	number int,  -- ������ʾʹ�õ���������ʾ��ȡ��������ϵͳ���ͣ�Ҳ�����ǹ���
	remark varchar(255),
	primary key(ID)
);

create index use_prop_index on prop_billes(student_id);

-- ��12������ ���ñ�
create table gems(
	ID int,
	name   varchar(255),
	photo  varchar(255),
	remark varchar(255),
	gem_rank int, 
	primary key(ID)
);

-- ��13����ȡ�����¼ ��̬��
create table gem_records(
	ID int,
	student_id int,
	gem_id int,
	reason varchar(255), -- ���ԭ������һ��ѧ������ô��õģ������Щ����
	get_time datetime,
	primary key(ID)
);

create index get_gem_index on gem_records(student_id);

-- ��14����״ ���ñ�
create table certis(
	ID int,
	name varchar(255),
	photo varchar(255),
	remark varchar(255),
	primary key(ID)
);

-- ��15��ѧ����ȡ��״��¼ ��̬��
create table certi_gets(
	ID int,
	student_id int,
	certi_id int,
	reason  varchar(255), -- ���ԭ������һ��ѧ������ô��õģ��ȼ������ļ������������ʲô����
	get_time datetime,
	primary key(ID)
);

create index get_certi_index on certi_gets(student_id);

-- ��16���ʼĽ�״��¼  ��̬��
create table certi_mails(
	ID int,
	student_id int,
	certi_id int,
	reason  varchar(255), -- ���ԭ������һ��ѧ������ô��õģ��ȼ������ļ������������ʲô����
    mail_time datetime,
	primary key(ID)
);

create index mail_certi_index on certi_mails(student_id);

--------------------------------------------------------------------------------------------------------
-- ��17����������  ��̬��
create table student_coins(
	ID int,
	student_id int,    -- ѧ��ID
	iscoin int default 0, 
	svcoin int default 0, 
	primary key(ID)
);

-- ��18���ֱ��˵�  ��̬��
create table iscoin_billes(
	ID int,
	student_id int, -- ѧ��ID
	iscoin int,     -- ������ʾ���룬������ʾ���
	good_id int,    -- �������ƷID��
	good_type varchar(255), -- ��Ʒ����
	remark varchar(2560), 
	op_time datetime,
	primary key(ID)
);

-- ��19�������˵�  ��̬��
create table silber_billes(
	ID int,
	student_id int, -- ѧ��ID
	svcoin int,     -- ������ʾ���룬������ʾ���
	good_id int,    -- �������ƷID��
	good_type varchar(255), -- ��Ʒ����
	remark varchar(2560), 
	op_time datetime,
	primary key(ID)
);

-- ��20����ֵ��¼  ��̬��
create table rechanges(
	ID int,
	student_id int,    -- ѧ��ID
	iscoin int, 
	op_time datetime,
	primary key(ID)
);

-- ��21���һ���¼  ��̬��
create table exchanges(
	ID int,
	student_id int,    -- ѧ��ID
	silvercoin int, 
	op_time datetime,
	primary key(ID)
);

