-- ѧ��ǩ��
-- ǩ��ʱ����ǩ����student_signs�в���һ�����ݣ�
--     ���޸�ѧ����students�����ǩ��ʱ��
--     ͬʱ������ǩ��������1

create procedure student_checkin
@student_id varchar(50), 
@curr_time bigint,  -- ǩ��ʱ��ʱ���,��ȷ����
@yesteday bigint
as
begin
insert student_checkins (student_id, checkin_time) values (@student_id, @curr_time);

declare @last_checkin bigint;
set @last_checkin = (select last_checkin from students where id = @student_id);
update students set last_checkin = @curr_time where id = @student_id;

if ( @last_checkin >= @yesteday )
update students set checkin_days = checkin_days + 1 where id = @student_id;
else
update students set checkin_days = 1 where id = @student_id;

end
