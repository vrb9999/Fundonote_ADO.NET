--creating user table
create table Users( 
UserId int identity(1,1) primary key,
Firstname varchar(50),
Lastname varchar(50),
Email varchar(50) unique,
Password varchar(255),
CreateDate datetime Default sysdatetime(),
MoidifyDate datetime Default getdate()   --sysdatetime and getdate will give same output 
)

insert into Users(Firstname,Lastname,Email,Password) values('Vinay','Biradar','vinay@gmail.com','Vinay@999')
select * from Users

--Created Stored Procedure
create procedure spAddUser(
@Firstname varchar(50), 
@Lastname varchar(50),
@Email varchar(50),
@password varchar(255)
)
As
Begin try
insert into Users(Firstname,Lastname,Email,Password) values(@Firstname,@Lastname,@Email,@password)
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

--executing the spAddUser stored procedure
exec spAddUser 'Suresh','kumar','suresh@gmail.com','Suresh@123'

--creating stored Procedure for Fetching User info from DB
create procedure spGetAllUser
As
Begin try
select * from Users
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

exec spGetAllUser

--Creating StoredProcedure For UserLogin Operation
create procedure spUserLogin(
@Email varchar(50),
@Password varchar(255)
)

As 
BEGIN TRY
select * from Users where Email=@Email and Password=@Password
END TRY
BEGIN CATCH
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

exec spUserLogin 'vinay@gmail.com','Vinay@999'

--Creating Stored Procedure For ForgetPassword api
create procedure spUserForgetPassword(
@Email varchar(50)
)
AS
BEGIN TRY
select * from Users where Email=@Email
END TRY
BEGIN CATCH
SELECT
      ERROR_NUMBER() AS ErrorNumber,
	  ERROR_STATE() AS ErrorState,
	  ERROR_PROCEDURE() AS ErrorProcedure,
	  ERROR_LINE() AS ErrorLine,
	  ERROR_MESSAGE() AS ErrorMessage
END CATCH

exec spUserForgetPassword 'vinay@gmail.com'

create procedure spResetPassword(
@Email varchar(50),
@Password varchar(50)
)
AS
BEGIN TRY
update Users set Password=@Password where Email=@Email
END TRY
BEGIN CATCH
SELECT
      ERROR_NUMBER() AS ErrorNumber,
	  ERROR_STATE() AS ErrorState,
	  ERROR_PROCEDURE() AS ErrorProcedure,
	  ERROR_LINE() AS ErrorLine,
	  ERROR_MESSAGE() AS ErrorMessage
END CATCH

exec spResetPassword 'vinay@gmail.com', 'Vinay@111'

Create procedure spResetPassword(
@Email varchar(50),
@Password varchar(50)
)
As
Begin try
select * from Users where Email=@Email 
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

select * from Users