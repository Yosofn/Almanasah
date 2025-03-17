ALTER TABLE Courses
ALTER COLUMN YearId INT NULL;

INSERT INTO Courses (Name, Descryption, Date, Price, [Order], TeacherId)
VALUES
('Mathematics Basics', 'Introduction to basic math concepts.', GETDATE(), 50.00, 1, 101),
('Science Fundamentals', 'Comprehensive course on fundamental science topics.', GETDATE(), 70.00, 2, 102),
('English Literature', 'Study of classical English literature.', GETDATE(), 60.00, 3, 103),
('Computer Science 101', 'Beginner programming and computer concepts.', GETDATE(), 80.00, 4, 104),
('Physics for Beginners', 'Learn the basics of physics and its applications.', GETDATE(), 55.00, 5, 101);
use almanasah

CREATE TABLE Units (
    Id INT IDENTITY(1,1) PRIMARY KEY,    -- Primary key for Units table
    Name NVARCHAR(255) NOT NULL,             -- Unit name
    Description NVARCHAR(MAX) NULL,          -- Optional unit description
    CourseId INT NOT NULL,                   -- Foreign key referencing Courses
    OrderNumber INT NOT NULL,                -- To maintain order within a course
    CONSTRAINT FK_Units_Course FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);
ALTER TABLE Lectures
drop column UnitId
ADD UnitId INT NOT NULL,
    CONSTRAINT FK_Lectures_Unit FOREIGN KEY (UnitId) REFERENCES Units(Id) ON DELETE CASCADE;
	select * from Lectures
	-- Insert data into the Units table
INSERT INTO Units (Name, Description, CourseId, OrderNumber)
VALUES
('Unit 1: Algebra Basics', 'Introduction to algebra concepts.', 3, 1), -- Assuming CourseId = 1 exists
('Unit 2: Geometry Basics', 'Introduction to geometry principles.', 3, 2),
('Unit 1: Programming Basics', 'Learn the fundamentals of programming.', 3, 1), -- Assuming CourseId = 2 exists
('Unit 2: Advanced Programming', 'Dive deeper into programming concepts.', 3, 2);

select * from Lectures
-- Insert data into the Lectures table
INSERT INTO Lectures (Name, Descryption, UnitId, [Order])
VALUES
-- Lectures for UnitId = 1 (Algebra Basics)
('Lecture 1: Variables and Expressions', 'Understanding variables and expressions.', 2, 1),
('Lecture 2: Solving Equations', 'Learn how to solve algebraic equations.', 2, 2),

-- Lectures for UnitId = 2 (Geometry Basics)
('Lecture 1: Lines and Angles', 'Basics of lines and angles in geometry.', 3, 1),
('Lecture 2: Polygons', 'Introduction to polygons and their properties.', 3, 2),

-- Lectures for UnitId = 3 (Programming Basics)
('Lecture 1: Introduction to Programming', 'What is programming and how it works.', 4, 1),
('Lecture 2: Variables in Programming', 'Learn how variables are used in programming.', 4, 2),

-- Lectures for UnitId = 4 (Advanced Programming)
('Lecture 1: Data Structures', 'Introduction to data structures in programming.', 4, 1),
('Lecture 2: Algorithms', 'Basics of algorithms and their applications.', 4, 2);
