if (not exists (select 1 from sys.databases where name = 'Securrency_TDS'))
begin
    create database Securrency_TDS
end
go

if (not exists (select * from sys.sql_logins where name = 'Securrency_TDS_App'))
begin
    use Securrency_TDS
    create login Securrency_TDS_App with password='1QAZ2wsx3EDC', default_database=Securrency_TDS
    create user Securrency_TDS_App from login Securrency_TDS_App
    exec sys.sp_addrolemember db_datareader, Securrency_TDS_App
    exec sys.sp_addrolemember db_datawriter, Securrency_TDS_App
end

if (not exists (select 1 from sys.sql_logins where name = 'Securrency_TDS_Adm'))
begin
    use Securrency_TDS
    create login Securrency_TDS_Adm with password='1QAZ2wsx3EDC', default_database=Securrency_TDS
    create user Securrency_TDS_Adm from login Securrency_TDS_Adm
    exec sys.sp_addrolemember db_datareader, Securrency_TDS_Adm
    exec sys.sp_addrolemember db_datawriter, Securrency_TDS_Adm
    exec sys.sp_addrolemember db_ddladmin, Securrency_TDS_Adm
end