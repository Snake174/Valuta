-- phpMyAdmin SQL Dump
-- version 5.1.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1
-- Время создания: Авг 18 2021 г., 05:33
-- Версия сервера: 10.4.19-MariaDB
-- Версия PHP: 7.4.19

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `val`
--

DELIMITER $$
--
-- Функции
--
CREATE DEFINER=`root`@`localhost` FUNCTION `GET_VAL_CURSE` (`val` VARCHAR(10), `dt` DATE) RETURNS DECIMAL(10,4) BEGIN
  DECLARE v DECIMAL(10,4);
  
  SELECT vc.value
    INTO v
    FROM val_curse vc
   WHERE vc.id_val = val
     AND vc.date = dt;
  
  RETURN v;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `val_curse`
--

CREATE TABLE `val_curse` (
  `id` int(11) NOT NULL,
  `id_val` varchar(3) NOT NULL,
  `date` date NOT NULL,
  `value` decimal(10,4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Структура таблицы `val_info`
--

CREATE TABLE `val_info` (
  `id` int(11) NOT NULL,
  `id_code` varchar(10) NOT NULL,
  `name` varchar(100) NOT NULL,
  `eng_name` varchar(100) NOT NULL,
  `nominal` int(11) NOT NULL,
  `parent_code` varchar(10) NOT NULL,
  `iso_num_code` int(11) NOT NULL,
  `iso_char_code` varchar(3) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `val_curse`
--
ALTER TABLE `val_curse`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `date` (`date`) USING BTREE;

--
-- Индексы таблицы `val_info`
--
ALTER TABLE `val_info`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id_code` (`id_code`) USING BTREE;

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `val_curse`
--
ALTER TABLE `val_curse`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `val_info`
--
ALTER TABLE `val_info`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
