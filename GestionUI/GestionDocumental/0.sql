-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: gestiondocumental
-- ------------------------------------------------------
-- Server version	5.7.17-log

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
-- Table structure for table `aprobacion_trd`
--

DROP TABLE IF EXISTS `aprobacion_trd`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aprobacion_trd` (
  `version` int(11) NOT NULL,
  `usuario` varchar(128) NOT NULL,
  `aprueba` tinyint(4) NOT NULL DEFAULT '0',
  `fecha_aprobacion` datetime DEFAULT NULL,
  PRIMARY KEY (`version`,`usuario`),
  KEY `user_aprobacion_idx` (`usuario`),
  CONSTRAINT `user_aprobacion` FOREIGN KEY (`usuario`) REFERENCES `aspnetusers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `version_aprobacion` FOREIGN KEY (`version`) REFERENCES `versiontrd` (`version`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aprobacion_trd`
--

LOCK TABLES `aprobacion_trd` WRITE;
/*!40000 ALTER TABLE `aprobacion_trd` DISABLE KEYS */;
/*!40000 ALTER TABLE `aprobacion_trd` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetroles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(256) NOT NULL,
  `aprobarTRD` tinyint(4) NOT NULL DEFAULT '0',
  `modificarTRD` tinyint(4) NOT NULL DEFAULT '0',
  `role_manager` tinyint(4) NOT NULL DEFAULT '0',
  `user_manager` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES (1,'Admin',1,1,1,1),(6,'Alcaldía',0,0,0,1);
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(128) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ApplicationUser_Claims` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `UserId` varchar(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`,`UserId`),
  KEY `ApplicationUser_Logins` (`UserId`),
  CONSTRAINT `ApplicationUser_Logins` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(128) NOT NULL,
  `RoleId` int(11) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityRole_Users` (`RoleId`),
  CONSTRAINT `ApplicationUser_Roles` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `ApplicatrionRoles_User` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('b7c544e3-f2f7-4f83-8819-f547d687f673',1),('f7cdfab0-622a-424d-b528-628c7c33f7bb',1);
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(128) NOT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEndDateUtc` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `UserName` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('b7c544e3-f2f7-4f83-8819-f547d687f673','joma501@hotmail.com',1,'ACgcYn9R8VxKT56RuGiRiO3K/J4MD8zBvdCSUL/3kRfvdTItWG0jXAYF6CIWX+dEBg==','236e3fcc-fd50-4031-a9b8-5278e8b9921c','',0,0,NULL,1,0,'joma501@hotmail.com'),('f7cdfab0-622a-424d-b528-628c7c33f7bb','davidsoto94@hotmail.com',1,'APl7Sno4jjla54gdaKQeQu/whAupYI1ZcZTwl1PzieCURajtEkv8P7wBK9/h9UN2kw==','12519a85-4430-4c59-9bb4-2441ed6fffd6','',0,0,NULL,1,0,'davidsoto94@hotmail.com');
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cargo`
--

DROP TABLE IF EXISTS `cargo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cargo` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cargo` varchar(70) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cargo`
--

LOCK TABLES `cargo` WRITE;
/*!40000 ALTER TABLE `cargo` DISABLE KEYS */;
/*!40000 ALTER TABLE `cargo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documento`
--

DROP TABLE IF EXISTS `documento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `documento` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_tipologia` int(11) NOT NULL,
  `año_rad` year(4) DEFAULT NULL,
  `cons_radicado` int(11) DEFAULT NULL,
  `respuesta` int(11) DEFAULT NULL,
  `observaciones` varchar(400) DEFAULT NULL,
  `folios` int(11) NOT NULL,
  `id_expediente` int(11) NOT NULL,
  `direccion` varchar(300) NOT NULL,
  `f_subida` datetime NOT NULL,
  `usuarioID` varchar(128) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `expediente_documento_idx` (`id_expediente`),
  KEY `ususario_documento_idx` (`usuarioID`),
  KEY `radicado_documento_idx` (`cons_radicado`,`año_rad`),
  CONSTRAINT `expediente_documento` FOREIGN KEY (`id_expediente`) REFERENCES `expediente` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `radicado_documento` FOREIGN KEY (`cons_radicado`, `año_rad`) REFERENCES `radicado` (`consecutivo`, `año`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `usuario_documento` FOREIGN KEY (`usuarioID`) REFERENCES `aspnetusers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documento`
--

LOCK TABLES `documento` WRITE;
/*!40000 ALTER TABLE `documento` DISABLE KEYS */;
/*!40000 ALTER TABLE `documento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `expediente`
--

DROP TABLE IF EXISTS `expediente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `expediente` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_trd` int(11) NOT NULL,
  `año` year(4) NOT NULL,
  `consecutivo_radicado` int(11) DEFAULT NULL,
  `f_creacion` datetime DEFAULT NULL,
  `f_cierre` datetime DEFAULT NULL,
  `usuario_creacion` varchar(160) DEFAULT NULL,
  `usuario_cierre` varchar(160) DEFAULT NULL,
  `observaciones` varchar(400) DEFAULT NULL,
  `identificacion` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `radicado_expediente_idx` (`consecutivo_radicado`),
  KEY `usuario_creacion_expediente_idx` (`usuario_creacion`),
  KEY `usuario_cierre_expediente_idx` (`usuario_cierre`),
  CONSTRAINT `radicado_expediente` FOREIGN KEY (`consecutivo_radicado`) REFERENCES `radicado` (`consecutivo`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `usuario_cierre_expediente` FOREIGN KEY (`usuario_cierre`) REFERENCES `aspnetusers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `usuario_creacion_expediente` FOREIGN KEY (`usuario_creacion`) REFERENCES `aspnetusers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `expediente`
--

LOCK TABLES `expediente` WRITE;
/*!40000 ALTER TABLE `expediente` DISABLE KEYS */;
/*!40000 ALTER TABLE `expediente` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notificaciones`
--

DROP TABLE IF EXISTS `notificaciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `notificaciones` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userID` varchar(120) NOT NULL,
  `notificacion` varchar(120) NOT NULL,
  `fecha` datetime NOT NULL,
  `leido` tinyint(4) DEFAULT '0',
  `url` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `user_notificaciones_idx` (`userID`),
  CONSTRAINT `user_notificaciones` FOREIGN KEY (`userID`) REFERENCES `aspnetusers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notificaciones`
--

LOCK TABLES `notificaciones` WRITE;
/*!40000 ALTER TABLE `notificaciones` DISABLE KEYS */;
/*!40000 ALTER TABLE `notificaciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `oficina`
--

DROP TABLE IF EXISTS `oficina`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `oficina` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(70) NOT NULL,
  `codigo` int(11) NOT NULL,
  `version` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `version` (`version`,`codigo`),
  UNIQUE KEY `version_2` (`version`,`codigo`),
  KEY `verion_oficina_idx` (`version`),
  CONSTRAINT `verion_oficina` FOREIGN KEY (`version`) REFERENCES `versiontrd` (`version`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `oficina`
--

LOCK TABLES `oficina` WRITE;
/*!40000 ALTER TABLE `oficina` DISABLE KEYS */;
/*!40000 ALTER TABLE `oficina` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `organizacion`
--

DROP TABLE IF EXISTS `organizacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `organizacion` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `organizacion`
--

LOCK TABLES `organizacion` WRITE;
/*!40000 ALTER TABLE `organizacion` DISABLE KEYS */;
INSERT INTO `organizacion` VALUES (1,'Año'),(2,'Identificacion'),(3,'Radicado'),(4,'Año con identificacion');
/*!40000 ALTER TABLE `organizacion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `radicado`
--

DROP TABLE IF EXISTS `radicado`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `radicado` (
  `consecutivo` int(11) NOT NULL,
  `año` year(4) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `email` varchar(60) DEFAULT NULL,
  `identificacion` bigint(20) DEFAULT NULL,
  `fecha` datetime NOT NULL,
  `entrada` tinyint(4) NOT NULL DEFAULT '0',
  `interno` tinyint(4) NOT NULL DEFAULT '1',
  `f_limite` date DEFAULT NULL,
  `numero_radicado` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`consecutivo`,`año`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `radicado`
--

LOCK TABLES `radicado` WRITE;
/*!40000 ALTER TABLE `radicado` DISABLE KEYS */;
/*!40000 ALTER TABLE `radicado` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `gestiondocumental`.`radi_BEFORE_INSERT` BEFORE INSERT ON `radicado` FOR EACH ROW
BEGIN
declare id_count int(11);
select if(count(consecutivo)=0,1,count(consecutivo)+1) from radicado
where año=new.año into id_count;

set new.consecutivo=id_count;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `role_manager`
--

DROP TABLE IF EXISTS `role_manager`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `role_manager` (
  `primario` int(11) NOT NULL,
  `secundario` int(11) NOT NULL,
  `agregarUsuario` tinyint(4) DEFAULT '0',
  PRIMARY KEY (`primario`,`secundario`),
  KEY `secundaria_idx` (`secundario`),
  CONSTRAINT `roles_primario` FOREIGN KEY (`primario`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `roles_secundario` FOREIGN KEY (`secundario`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_manager`
--

LOCK TABLES `role_manager` WRITE;
/*!40000 ALTER TABLE `role_manager` DISABLE KEYS */;
INSERT INTO `role_manager` VALUES (1,1,1),(1,6,1),(6,6,1);
/*!40000 ALTER TABLE `role_manager` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role_trd`
--

DROP TABLE IF EXISTS `role_trd`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `role_trd` (
  `role_id` int(11) NOT NULL,
  `trd_id` int(11) NOT NULL,
  PRIMARY KEY (`role_id`,`trd_id`),
  KEY `trd_role_trd_idx` (`trd_id`),
  CONSTRAINT `rol_rol_trd` FOREIGN KEY (`role_id`) REFERENCES `aspnetroles` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `trd_role_trd` FOREIGN KEY (`trd_id`) REFERENCES `trd` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role_trd`
--

LOCK TABLES `role_trd` WRITE;
/*!40000 ALTER TABLE `role_trd` DISABLE KEYS */;
/*!40000 ALTER TABLE `role_trd` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `serie`
--

DROP TABLE IF EXISTS `serie`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `serie` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `codigo` int(11) NOT NULL,
  `versiontrd` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `versiontrd` (`versiontrd`,`codigo`),
  KEY `version_serie_idx` (`versiontrd`),
  CONSTRAINT `version_serie` FOREIGN KEY (`versiontrd`) REFERENCES `versiontrd` (`version`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `serie`
--

LOCK TABLES `serie` WRITE;
/*!40000 ALTER TABLE `serie` DISABLE KEYS */;
/*!40000 ALTER TABLE `serie` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `subserie`
--

DROP TABLE IF EXISTS `subserie`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `subserie` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(110) NOT NULL,
  `codigo` int(11) NOT NULL,
  `gestion` int(11) DEFAULT NULL,
  `versiontrd` int(11) NOT NULL,
  `archivo` int(11) NOT NULL,
  `d_final` varchar(15) NOT NULL,
  `observaciones` varchar(300) DEFAULT NULL,
  `identificacion` varchar(45) DEFAULT NULL,
  `id_organizacion` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `versiontrd` (`versiontrd`,`codigo`),
  KEY `version_subserie_idx` (`versiontrd`),
  KEY `sdfsfdf_idx` (`id_organizacion`),
  CONSTRAINT `org_subserie` FOREIGN KEY (`id_organizacion`) REFERENCES `organizacion` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `version_subserie` FOREIGN KEY (`versiontrd`) REFERENCES `versiontrd` (`version`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `subserie`
--

LOCK TABLES `subserie` WRITE;
/*!40000 ALTER TABLE `subserie` DISABLE KEYS */;
/*!40000 ALTER TABLE `subserie` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipologia`
--

DROP TABLE IF EXISTS `tipologia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tipologia` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) NOT NULL,
  `id_trd` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_trd` (`id_trd`,`nombre`),
  KEY `trd_tipologia_idx` (`id_trd`),
  CONSTRAINT `trd_tipologia` FOREIGN KEY (`id_trd`) REFERENCES `trd` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipologia`
--

LOCK TABLES `tipologia` WRITE;
/*!40000 ALTER TABLE `tipologia` DISABLE KEYS */;
/*!40000 ALTER TABLE `tipologia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trd`
--

DROP TABLE IF EXISTS `trd`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `trd` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_oficina` int(11) NOT NULL,
  `id_serie` int(11) NOT NULL,
  `id_subserie` int(11) NOT NULL,
  `version` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_oficina` (`id_oficina`,`id_serie`,`id_subserie`,`version`),
  KEY `oficina_trd_idx` (`id_oficina`),
  KEY `serie_trd_idx` (`id_serie`),
  KEY `version_trd_idx` (`version`),
  KEY `subserie_trd_idx` (`id_subserie`),
  CONSTRAINT `oficina_trd` FOREIGN KEY (`id_oficina`) REFERENCES `oficina` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `serie_trd` FOREIGN KEY (`id_serie`) REFERENCES `serie` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `subserie_trd` FOREIGN KEY (`id_subserie`) REFERENCES `subserie` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `version_trd` FOREIGN KEY (`version`) REFERENCES `versiontrd` (`version`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trd`
--

LOCK TABLES `trd` WRITE;
/*!40000 ALTER TABLE `trd` DISABLE KEYS */;
/*!40000 ALTER TABLE `trd` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_details`
--

DROP TABLE IF EXISTS `user_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user_details` (
  `id` varchar(128) NOT NULL,
  `nombre` varchar(60) DEFAULT NULL,
  `apellido` varchar(60) DEFAULT NULL,
  `identificacion` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `user_details` FOREIGN KEY (`id`) REFERENCES `aspnetusers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_details`
--

LOCK TABLES `user_details` WRITE;
/*!40000 ALTER TABLE `user_details` DISABLE KEYS */;
INSERT INTO `user_details` VALUES ('b7c544e3-f2f7-4f83-8819-f547d687f673','Jose ','Echeverry',1098657178),('f7cdfab0-622a-424d-b528-628c7c33f7bb','David','Soto',1098751786);
/*!40000 ALTER TABLE `user_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `versiontrd`
--

DROP TABLE IF EXISTS `versiontrd`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `versiontrd` (
  `version` int(11) NOT NULL AUTO_INCREMENT,
  `fecha_creacion` date NOT NULL,
  `usuario_modificador` varchar(128) DEFAULT NULL,
  `aprobado` datetime DEFAULT NULL,
  PRIMARY KEY (`version`),
  KEY `User_verionTRD_idx` (`usuario_modificador`),
  CONSTRAINT `User_verionTRD` FOREIGN KEY (`usuario_modificador`) REFERENCES `aspnetusers` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `versiontrd`
--

LOCK TABLES `versiontrd` WRITE;
/*!40000 ALTER TABLE `versiontrd` DISABLE KEYS */;
/*!40000 ALTER TABLE `versiontrd` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'gestiondocumental'
--

--
-- Dumping routines for database 'gestiondocumental'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-12-28  9:50:10
