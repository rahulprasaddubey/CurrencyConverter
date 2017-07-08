
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/08/2017 16:49:09
-- Generated from EDMX file: E:\MVC\CurrencyConverter\CurrencyConverter\RateExchange.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Converter];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CurrencyExchangeRates'
CREATE TABLE [dbo].[CurrencyExchangeRates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Currency] nvarchar(max)  NOT NULL,
    [RateInINR] float  NOT NULL,
    [RowUpdatedDate] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'CurrencyExchangeRates'
ALTER TABLE [dbo].[CurrencyExchangeRates]
ADD CONSTRAINT [PK_CurrencyExchangeRates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------