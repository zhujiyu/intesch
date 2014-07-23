create proc get_student_login
@student_id varchar(50)
as
select top 1 id, student_id, login_time, logout_time, ipaddress, client_type 
from student_logins
where student_id = @student_id
order by login_time desc
