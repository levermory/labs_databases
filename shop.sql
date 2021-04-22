drop database if exists shop;
create database shop;
use shop;

create table manufact
(
    id      int primary key,
    name    varchar(50),
    country varchar(20),
    phone   varchar(20),
    address varchar(50)
);

insert into manufact
    ()
values (1, 'Alivaria', 'Belarus', '+375297520012', 'Minsk, Kiseleva 30'),
       (2, 'Krynitsa', 'Belarus', '+375337805611', 'Minsk, Radialnaya 52'),
       (3, 'Bulbash', 'Belarus', '+375296000033', 'Minsk, Nezavisimosti 4'),
       (4, 'Efes', 'Russia', 'phone not included', 'Russia, Somewhere'),
       (5, 'AllWinesPridumalCompany', 'Georgia', 'zvonite Natashe', 'Georgia, Somewhere');

create table product
(
    id       int primary key,
    name     varchar(30),
    type     enum ('beer', 'vodka'),
    manuf_id int,
    quantity int,
    price    double,
    constraint manuf_id_ref foreign key (manuf_id) references manufact (id) on delete cascade
);

insert into product
    ()
values (1, 'Alivaria 10', 1, 1, 10, 4.99),
       (2, 'Porter', 1, 1, 13, 5.99),
       (3, 'Mocnae', 1, 2, 100, 3.59),
       (4, 'Bulbash special', 2, 3, 22, 19.99),
       (5, 'Bulbash classic', 2, 3, 11, 14.99);

create table customer
(
    id   int primary key,
    name varchar(50)
);

insert into customer
    ()
values (1, 'Natasha'),
       (2, 'Edgar'),
       (3, 'Pasha'),
       (4, 'Anna');

create table purchase
(
    id int primary key auto_increment,
    cust_id int,
    prod_id int,
    amount int,
    date datetime,
    constraint cust_id_ref foreign key (cust_id) references customer(id) on delete cascade ,
    constraint prod_if_ref foreign key (prod_id) references product(id) on delete cascade
);

INSERT INTO shop.purchase (cust_id, prod_id, amount, date)
VALUES (1, 1, 3, '2021-04-14 22:43:13'),
       (1, 2, 5, '2021-04-14 22:43:16'),
       (1, 3, 7, '2021-04-14 22:43:17'),
       (1, 4, 10, '2021-04-04 00:00:00'),
       (1, 5, 10, '2021-04-04 00:00:00'),
       (2, 1, 10, '2020-01-01 00:00:00'),
       (2, 3, 5, '2021-01-01 00:00:00'),
       (3, 1, 10, '2021-04-04 00:00:00'),
       (3, 2, 10, '2021-04-14 00:00:00'),
       (3, 3, 10, '2020-04-04 00:00:00'),
       (4, 4, 1, '2021-04-14 22:59:10'),
       (4, 5, 1, '2021-04-14 22:59:12');

