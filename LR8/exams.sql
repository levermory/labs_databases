drop database if exists exams;
create database exams;
use exams;

create table `subject`
(
    `id`      int primary key,
    `name`    varchar(50),
    `teacher` varchar(50),
    `hours`   int
);

create table `student`
(
    `id`     int primary key,
    `name`   varchar(50),
    `spec`   enum ('KM', 'WEB', 'MOB'),
    `form`   enum ('day', 'absent'),
    `value`  float,
    `salary` float,
    `course` tinyint(4)
);

create table `exam`
(
    `id`         int primary key,
    `date`       datetime,
    `subject_id` int,
    `student_id` int,
    `mark`       int,
    constraint subj_ref foreign key (subject_id) references subject (id),
    constraint stud_ref foreign key (student_id) references student (id)
);

insert into `subject`
    ()
values (1, 'Databases', 'Big Kush', 1000000),
       (2, 'Differential geometry', 'Basis', 1000),
       (3, 'Neural networks', 'Malevich', 2000);

insert into `student`
    ()
values (1, 'Levi', 'KM', 'day', 8.0, 130, 2),
       (2, 'Alex', 'MOB', 'absent', 7.6, 170, 1),
       (3, 'Jan', 'WEB', 'day', 10, 170, 2),
       (4, 'Chumur Turko', 'KM', 'day', 8.4, 150, 2);

delimiter ;;
create trigger valueCheck
    before insert
    on `student`
    for each row
begin
    if new.`value` < 1 or new.`value` > 10 then
        signal sqlstate '45000' set message_text = 'wrong value';
    end if;
end;;

delimiter ;

