CREATE DATABASE almanasah;
USE almanasah;

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(15),
    ParentPhone NVARCHAR(15),
    Government NVARCHAR(100),
    NationalId NVARCHAR(20) UNIQUE,
    RegisterDate DATETIME DEFAULT GETDATE(),
    UserType INT
);
CREATE TABLE Years (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Descryption NVARCHAR(255)
);

-- إنشاء جدول Courses
CREATE TABLE Courses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Descryption NVARCHAR(255),
    Date DATETIME,
    Price DECIMAL(10, 2),
    [Order] INT,
    TeacherId INT,
    YearId INT,
    FOREIGN KEY (YearId) REFERENCES Years(Id)
);

-- إنشاء جدول Lectures
CREATE TABLE Lectures (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Date DATETIME,
    Url NVARCHAR(255),
    Descryption NVARCHAR(255),
    [Order] INT,
    CourseId INT,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id)
);

CREATE TABLE UserCourses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT,
    CourseId INT,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CourseId) REFERENCES Courses(Id)
);

CREATE TABLE Packages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Descryption NVARCHAR(255),
    Price DECIMAL(10, 2)
);

CREATE TABLE CoursePackages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PackageId INT,
    CourseId INT,
    FOREIGN KEY (PackageId) REFERENCES Packages(Id),
    FOREIGN KEY (CourseId) REFERENCES Courses(Id)
);
ALTER TABLE Lectures
ADD TeacherId INT NULL,
    Price DECIMAL(10, 2) NULL;

ALTER TABLE Lectures
ADD CONSTRAINT FK_Lectures_TeacherId FOREIGN KEY (TeacherId) REFERENCES Users(Id);


CREATE TABLE UserLectures (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT,
    LectureId INT,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (LectureId) REFERENCES Lectures(Id)
);


