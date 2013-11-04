CREATE DATABASE  IF NOT EXISTS `supermaret` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `supermaret`;
-- MySQL dump 10.13  Distrib 5.6.12, for Win64 (x86_64)
--
-- Host: localhost    Database: supermaret
-- ------------------------------------------------------
-- Server version	5.6.12

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `measures`
--

DROP TABLE IF EXISTS `measures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `measures` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `MeasureName` varchar(45) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=501 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `measures`
--

LOCK TABLES `measures` WRITE;
/*!40000 ALTER TABLE `measures` DISABLE KEYS */;
INSERT INTO `measures` VALUES (100,'liters'),(200,'pieces'),(300,'grams'),(400,'ounces'),(500,'blister');
/*!40000 ALTER TABLE `measures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `products` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `ProductName` varchar(45) NOT NULL,
  `BasePrice` decimal(10,2) NOT NULL,
  `Measures_ID` int(11) NOT NULL,
  `Vendors_ID` int(11) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_Products_Measures_idx` (`Measures_ID`),
  KEY `fk_Products_Vendors1_idx` (`Vendors_ID`),
  CONSTRAINT `fk_Products_Measures` FOREIGN KEY (`Measures_ID`) REFERENCES `measures` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_Products_Vendors1` FOREIGN KEY (`Vendors_ID`) REFERENCES `vendors` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=55 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'Beer “Zagorka”',2.50,100,2),(2,'Vodka “Targovishte”',8.34,100,3),(3,'Beer “Beck’s”',3.00,100,2),(4,'Chocolate “Milka”',2.55,200,1),(5,'Beer Amstel',2.30,100,2),(6,'Kit kat',1.20,200,1),(7,'Hamburger',3.00,200,8),(8,'Baba Milk',1.00,100,15),(9,'Mineral Water Devin',0.65,100,19),(10,'Cheese Fantastiko',3.45,300,18),(11,'Beer Pirinsko 5l.',5.20,100,14),(12,'Pan Tefal',30.00,200,16),(13,'Karnobatska Grozdova',15.50,100,13),(14,'Pepsi 0.5l.',1.20,100,7),(15,'Pepsi 3.5l.',3.00,100,7),(16,'Jim Beam 0.7l.',30.00,100,10),(17,'Jim Beam 1l.',40.00,100,10),(18,'Jack Daniels 0.7l.',40.00,100,9),(19,'Jack Daniels 1l.',50.00,100,9),(20,'Bread Billa',1.00,300,17),(21,'Trainers Adidas',100.00,200,20),(22,'Airfresher Aro',2.00,200,4),(23,'Beer Pirinsko 2.5l.',3.00,100,14),(24,'Coca-Cola 0.5l.',1.20,100,6),(25,'Coca-Cola 1.5l.',2.50,100,6),(26,'Coca-Cola 1.5l.',2.50,100,6),(27,'Beer “Zagorka”',2.50,100,2),(28,'Vodka “Targovishte”',8.34,100,3),(29,'Beer “Beck’s”',3.00,100,2),(30,'Chocolate “Milka”',2.55,200,1),(31,'Beer Amstel',2.30,100,2),(32,'Kit kat',1.20,200,1),(33,'Hamburger',3.00,200,8),(34,'Baba Milk',1.00,100,15),(35,'Mineral Water Devin',0.65,100,19),(36,'Cheese Fantastiko',3.45,300,18),(37,'Beer Pirinsko 0.5l.',1.20,100,14),(38,'Pan Tefal',30.00,200,16),(39,'Karnobatska Grozdova',15.50,100,13),(40,'Pepsi 0.5l.',1.20,100,7),(41,'Pepsi 2l.',3.00,100,7),(42,'Jim Beam 0.7l.',30.00,100,10),(43,'Jim Beam 1l.',40.00,100,10),(44,'Jack Daniels 0.7l.',40.00,100,9),(45,'Jack Daniels 1l.',50.00,100,9),(46,'Bread Billa',1.00,300,17),(47,'Trainers Adidas',100.00,200,20),(48,'Airfresher Aro',2.00,200,4),(49,'Beer Pirinsko 2.5l.',3.00,100,14),(50,'Coca-Cola 0.5l.',1.20,100,6),(51,'Coca-Cola 1.5l.',2.50,100,6),(52,'Coca-Cola 2.5l.',3.50,100,6),(53,'Gum \"Turbo\"',0.24,500,5),(54,'Caffee \"Nova Brasilia\"',3.24,400,5);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vendors`
--

DROP TABLE IF EXISTS `vendors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendors` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `VendorName` varchar(45) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendors`
--

LOCK TABLES `vendors` WRITE;
/*!40000 ALTER TABLE `vendors` DISABLE KEYS */;
INSERT INTO `vendors` VALUES (1,'Nestle Sofia Corp.'),(2,'Zagorka Corp.'),(3,'Targovishte Bottling Company Ltd.'),(4,'Aro Ltd.'),(5,'Orbit Ltd.'),(6,'Coca-Cola Ltd.'),(7,'Pepsi Ltd.'),(8,'Mc Donalds  Ltd.'),(9,'Jack Daniels  Ltd.'),(10,'Jim Beam Ltd.'),(11,'Jagermeister Ltd.'),(12,'Peshtera AD'),(13,'Karnobat  Ltd.'),(14,'Carlsberg Ltd.'),(15,'Milk Sofia  Ltd.'),(16,'Tefal  Ltd.'),(17,'Clever Ltd.'),(18,'Fantastico Ltd.'),(19,'Devin'),(20,'Adidas');
/*!40000 ALTER TABLE `vendors` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-07-22 16:14:56
