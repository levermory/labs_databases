drop database if exists new_shop;
create database new_shop;
use new_shop;

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