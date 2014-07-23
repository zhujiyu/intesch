--if exists(select * from sysobjects where name = 'student_sign' and type = 'p')
--drop procedure student_sign;

create procedure student_sign
@student_id varchar(50), @curr_time datetime
as
begin
insert student_signs (student_id, sign_date) values (@student_id, @curr_time);
update students set last_sign = @curr_time where id = @student_id;
end

create proc student_login
@student_id varchar(50)
as 
begin
update students set last_sign = @curr_time where id = @student_id;
end