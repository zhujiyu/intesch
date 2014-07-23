create proc student_checkin_log
@student_id varchar(50)
as 
select id, student_id, DATEADD(S, checkin_time + 8 * 3600,'1970-01-01 00:00:00') as checkin_time 
from student_checkins
where student_id = @student_id;
