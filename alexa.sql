-- phpMyAdmin SQL Dump
-- version 4.8.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jan 11, 2019 at 10:53 AM
-- Server version: 10.1.33-MariaDB
-- PHP Version: 7.2.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `alexa`
--

-- --------------------------------------------------------

--
-- Table structure for table `accountdetails`
--

CREATE TABLE `accountdetails` (
  `Customerid` int(11) NOT NULL,
  `Name` text NOT NULL,
  `Age` int(11) NOT NULL,
  `Address` text NOT NULL,
  `Branch` text NOT NULL,
  `Password` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `accountdetails`
--

INSERT INTO `accountdetails` (`Customerid`, `Name`, `Age`, `Address`, `Branch`, `Password`) VALUES
(1, 'Chinmay', 20, 'Thakurli', 'Andheri', 1234);

-- --------------------------------------------------------

--
-- Table structure for table `buyintent`
--

CREATE TABLE `buyintent` (
  `srno` int(11) NOT NULL,
  `item` text NOT NULL,
  `amount` int(11) NOT NULL,
  `time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `buyintent`
--

INSERT INTO `buyintent` (`srno`, `item`, `amount`, `time`) VALUES
(1, 'mobile', 10000, '2019-01-01 06:17:27'),
(2, 'mobile', 10000, '2019-01-09 06:21:38'),
(3, 'car', 200000, '2019-01-09 06:58:53'),
(4, 'rice', 1000, '2019-01-09 07:52:29'),
(5, 'mobile', 10000, '2019-01-09 12:12:09');

-- --------------------------------------------------------

--
-- Table structure for table `transferintent`
--

CREATE TABLE `transferintent` (
  `srno` int(11) NOT NULL,
  `account` int(11) NOT NULL,
  `amount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `transferintent`
--

INSERT INTO `transferintent` (`srno`, `account`, `amount`) VALUES
(1, 3132, 1000),
(2, 91667, 3000),
(3, 91143, 3000),
(4, 1111, 100),
(5, 12233, 1000),
(6, 1234, 1000),
(7, 1234, 100),
(8, 4000, 3000),
(9, 1222, 1000);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `accountdetails`
--
ALTER TABLE `accountdetails`
  ADD PRIMARY KEY (`Customerid`);

--
-- Indexes for table `buyintent`
--
ALTER TABLE `buyintent`
  ADD PRIMARY KEY (`srno`);

--
-- Indexes for table `transferintent`
--
ALTER TABLE `transferintent`
  ADD PRIMARY KEY (`srno`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `buyintent`
--
ALTER TABLE `buyintent`
  MODIFY `srno` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `transferintent`
--
ALTER TABLE `transferintent`
  MODIFY `srno` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
