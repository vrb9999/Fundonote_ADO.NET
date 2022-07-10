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