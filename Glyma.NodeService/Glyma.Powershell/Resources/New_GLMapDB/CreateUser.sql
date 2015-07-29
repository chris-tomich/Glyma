/****** Object:  Login [ACCOUNT_NAME]    Script Date: 11/07/2012 12:00:00 PM ******/
IF NOT EXISTS 
	(SELECT * FROM  master.dbo.syslogins WHERE loginname = N'[ACCOUNT_NAME]')
	BEGIN
		CREATE LOGIN "[ACCOUNT_NAME]" FROM WINDOWS
	END


/****** Object:  User [ACCOUNT_NAME]    Script Date: 11/07/2012 12:00:00 PM ******/
IF NOT EXISTS 
    (SELECT sp.name AS ServerLoginName, dp.name AS DBUserName FROM sys.server_principals sp INNER JOIN sys.database_principals dp ON sp.sid = dp.sid WHERE sp.name = N'[ACCOUNT_NAME]')
	BEGIN
		IF NOT EXISTS
			(SELECT * FROM  dbo.sysusers WHERE name = N'[ACCOUNT_NAME]' AND uid < 16382)
			BEGIN
				CREATE USER "[ACCOUNT_NAME]"
			END
	END