-- 学生签到
-- 签到时，在签到表student_signs中插入一条数据，
--     并修改学生表students中最后签到时间
--     同时将连续签到次数加1

create procedure student_checkin
@student_id varchar(50), 
@curr_time bigint,  -- 签到时的时间戳,精确到秒
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
