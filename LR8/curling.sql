drop database if exists curling;
create database curling;
use curling;

create table player
(
    `id`     int primary key,
    `name`   varchar(50),
    `height` float,
    `weight` float,
    `age`    int,
    `exp`    int
);

create table team
(
    `id`       int primary key,
    `country`  varchar(30),
    `points`   int,
    `rating`   int
);

create table team_player
(
    team_id   int,
    player_id int,
    constraint team_id_ref foreign key (team_id) references team (id) on delete cascade,
    constraint player_id_ref foreign key (player_id) references player (id) on delete cascade
);

create table game
(
    id     int primary key auto_increment,
    place  varchar(30),
    date   date,
    team_1 int,
    team_2 int,
    winner int,
    constraint team1ref foreign key (team_1) references team (id),
    constraint team2ref foreign key (team_2) references team (id),
    constraint teamwinref foreign key (winner) references team (id)
);

delimiter //
create procedure updateRating()
begin
    declare is_end int default 0;
    declare _rating int;
    declare _id int;

    declare teams cursor for (select id from team order by points desc);
    declare continue handler for not found set is_end = 1;
    set _rating = 0;

    open teams;
    loop1 :
    loop
        fetch teams into _id;
        if is_end then
            leave loop1;
        end if;
        set _rating = _rating+1;
        select _rating;
        update team set team.rating = _rating where team.id = _id;
    end loop;
    close teams;
end //
create procedure newGame(in _winner int, in _looser int, in _place varchar(30), in _date date)
begin
    insert into game (place, date, team_1, team_2, winner) VALUE (_place, _date, _winner, _looser, _winner);
    update team set points = points + 240 where team.id = _winner;
    update team set points = points + 200 where team.id = _looser;
    call updateRating();
end //
delimiter ;
insert into curling.team (id, country, points, rating)
values (1, 'Sweden', 89020, 1),
       (2, 'Canada', 75980, 2),
       (3, 'USA', 71029, 3),
       (4, 'Switzerland', 65196, 4),
       (5, 'Schotland', 52059, 5);

insert into curling.player (id, name, height, weight, age, exp)
values (1, 'Nicklas Edin', null, null, 1, null),
       (2, 'Oscar Ericsson', null, null, 1, null),
       (4, 'John Shooster', null, null, 1, null),
       (3, 'Rasmus Vrano', null, null, 1, null),
       (5, 'Matt Hamilton', null, null, 1, null),
       (6, 'Kevin Koe', null, null, 1, null),
       (7, 'Colton Flash', null, null, 1, null),
       (8, 'Sven Mishelle', null, null, 1, null),
       (9, 'Peter de Cruz', null, null, 1, null),
       (10, 'Grant Hardi', null, null, 1, null),
       (11, 'Bobby Lemmi', null, null, 1, null);

insert into curling.team_player (team_id, player_id)
values (1, 1),
       (1, 3),
       (1, 2),
       (3, 4),
       (3, 5),
       (2, 6),
       (2, 7),
       (4, 8),
       (4, 9),
       (5, 10),
       (5, 11);

UPDATE curling.player t
SET t.height = 187,
    t.weight = 80
WHERE t.id = 3;

UPDATE curling.player t
SET t.height = 177
WHERE t.id = 11;

UPDATE curling.player t
SET t.height = 175,
    t.weight = 79
WHERE t.id = 7;

UPDATE curling.player t
SET t.height = 163,
    t.weight = 67
WHERE t.id = 4;

UPDATE curling.player t
SET t.height = 192,
    t.weight = 81
WHERE t.id = 6;

UPDATE curling.player t
SET t.height = 179,
    t.weight = 83
WHERE t.id = 8;

UPDATE curling.player t
SET t.height = 189,
    t.weight = 77
WHERE t.id = 9;

UPDATE curling.player t
SET t.height = 190,
    t.weight = 73
WHERE t.id = 2;

UPDATE curling.player t
SET t.height = 200,
    t.weight = 70
WHERE t.id = 5;

UPDATE curling.player t
SET t.height = 185,
    t.weight = 75
WHERE t.id = 1;

UPDATE curling.player t
SET t.height = 190,
    t.weight = 55
WHERE t.id = 10;

UPDATE curling.player t
SET t.height = 177,
    t.weight = 70
WHERE t.id = 11;

