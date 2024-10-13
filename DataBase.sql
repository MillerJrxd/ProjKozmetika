CREATE DATABASE kozmetika DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;

USE kozmetika;

CREATE TABLE Ügyfél (
  ugyfelID INT PRIMARY KEY AUTO_INCREMENT,
  ugyfelFirstName VARCHAR(50),
  ugyfelLastName VARCHAR(50),
  ugyfelTel VARCHAR(15),
  ugyfelEmail VARCHAR(100),
  ugyfelPontok INT
);
CREATE TABLE Dolgozók (
  dolgozoID INT PRIMARY KEY AUTO_INCREMENT,
  dolgozoFirstName VARCHAR(50),
  dolgozoLastName VARCHAR(50),
  dolgozoTel VARCHAR(15),
  dolgozoEmail VARCHAR(100),
  statusz BOOLEAN,
  szolgáltatasa INT
);
CREATE TABLE Szolgáltatás (
  szolgaltatasID INT PRIMARY KEY AUTO_INCREMENT,
  szolgaltatasKategoria VARCHAR(50),
  szolgaltatasIdotartam TIME,
  szolgaltatasAr INT
);
CREATE TABLE Foglalás (
  foglalasID INT PRIMARY KEY AUTO_INCREMENT,
  szolgaltatasID INT,
  dolgozoID INT,
  ugyfelID INT,
  foglalasStart DATETIME,
  foglalasEnd DATETIME,
  FOREIGN KEY (szolgaltatasID) REFERENCES Szolgáltatás(szolgaltatasID),
  FOREIGN KEY (dolgozoID) REFERENCES Dolgozók(dolgozoID),
  FOREIGN KEY (ugyfelID) REFERENCES Ügyfél(ugyfelID)
);
INSERT INTO Szolgáltatás (szolgaltatasKategoria, szolgaltatasIdotartam, szolgaltatasAr)
VALUES
('Masszázs', '00:30:00', 5000),
('Arcápolás', '00:45:00', 8000),
('Manikűr', '01:00:00', 6000),
('Pedikűr', '00:45:00', 7000),
('Hajvágás', '00:30:00', 4000),
('Szőrtelenítés', '00:45:00', 7500),
('Alakformálás', '01:30:00', 10000),
('Arcbőr fiatalítás', '02:00:00', 12000),
('Spa kezelés', '01:15:00', 9000),
('Körömlakk eltávolítás', '00:15:00', 3000);
INSERT INTO Dolgozók (dolgozoFirstName, dolgozoLastName, dolgozoTel, dolgozoEmail, statusz, szolgáltatasa) 
VALUES
('János', 'Kovács', '06301234567', 'janos.kovacs@example.com', true, 1),
('Anna', 'Nagy', '06304567890', 'anna.nagy@example.com', true, 3),
('Péter', 'Szabó', '06305551234', 'peter.szabo@example.com', true, 5),
('Zsuzsanna', 'Tóth', '06307894561', 'zsuzsanna.toth@example.com', true, 2),
('István', 'Horváth', '06305671234', 'istvan.horvath@example.com', true, 7),
('Éva', 'Kovács', '06309998877', 'eva.kovacs@example.com', true, 4),
('Gábor', 'Molnár', '06306661234', 'gabor.molnar@example.com', true, 6),
('Katalin', 'Szűcs', '06301112233', 'katalin.szucs@example.com', true, 8),
('László', 'Papp', '06308978765', 'laszlo.papp@example.com', true, 9),
('Mariann', 'Varga', '06304561234', 'mariann.varga@example.com', true, 10);
INSERT INTO Ügyfél (ugyfelFirstName, ugyfelLastName, ugyfelTel, ugyfelEmail, ugyfelPontok)
VALUES
('Ágnes', 'Kovács', '06301234567', 'agnes.kovacs@example.com', 350),
('Gergő', 'Nagy', '06304567890', 'gergo.nagy@example.com', 200),
('Eszter', 'Szabó', '06305551234', 'eszter.szabo@example.com', 450),
('Bence', 'Tóth', '06307894561', 'bence.toth@example.com', 600),
('Dóra', 'Horváth', '06305671234', 'dora.horvath@example.com', 800),
('Gábor', 'Kovács', '06309998877', 'gabor.kovacs@example.com', 550),
('Réka', 'Molnár', '06306661234', 'reka.molnar@example.com', 700),
('Márton', 'Szűcs', '06301112233', 'marton.szucs@example.com', 300),
('Petra', 'Papp', '06308978765', 'petra.papp@example.com', 400),
('Anna', 'Varga', '06304561234', 'anna.varga@example.com', 750);
INSERT INTO Foglalás (szolgaltatasID, dolgozoID, ugyfelID, foglalasStart, foglalasEnd)
VALUES
(1, 1, 1, '2024-10-15 10:00:00', '2024-10-15 10:30:00'),
(2, 2, 2, '2024-10-16 14:00:00', '2024-10-16 14:45:00'),
(3, 3, 3, '2024-10-17 11:30:00', '2024-10-17 12:30:00'),
(4, 4, 4, '2024-10-18 09:00:00', '2024-10-18 09:45:00'),
(5, 5, 5, '2024-10-19 16:00:00', '2024-10-19 16:30:00'),
(6, 6, 6, '2024-10-20 13:00:00', '2024-10-20 13:45:00'),
(7, 7, 7, '2024-10-21 15:00:00', '2024-10-21 16:30:00'),
(8, 8, 8, '2024-10-22 10:30:00', '2024-10-22 12:30:00'),
(9, 9, 9, '2024-10-23 11:00:00', '2024-10-23 12:15:00'),
(10, 10, 10, '2024-10-24 14:00:00', '2024-10-24 14:15:00');



