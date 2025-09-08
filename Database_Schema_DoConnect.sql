CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) DEFAULT 'User'
);

CREATE TABLE Questions (
    QuestionsId INT PRIMARY KEY IDENTITY,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    QuestionTitle NVARCHAR(255) NOT NULL,
    QuestionText NVARCHAR(MAX),
    Status BIT NOT NULL
);

CREATE TABLE Answers (
    AnswersId INT PRIMARY KEY IDENTITY,
    QuestionsId INT FOREIGN KEY REFERENCES Questions(QuestionsId),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    AnswersText NVARCHAR(MAX) NOT NULL,
    Status BIT NOT NULL
);

CREATE TABLE Images (
    ImagesId INT PRIMARY KEY IDENTITY,
    ImagePath NVARCHAR(255) NOT NULL,
    QuestionsId INT NULL FOREIGN KEY REFERENCES Questions(QuestionsId),
    AnswersId INT NULL FOREIGN KEY REFERENCES Answers(AnswersId)
);
