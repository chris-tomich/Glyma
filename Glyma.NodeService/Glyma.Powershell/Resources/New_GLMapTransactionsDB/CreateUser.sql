/****** Object:  Login [ACCOUNT_NAME]    Script Date: 11/07/2012 12:00:00 PM ******/
IF NOT EXISTS 
	(SELECT * FROM  master.dbo.syslogins WHERE loginname = N'[ACCOUNT_NAME]')
BEGIN
	CREATE LOGIN "[ACCOUNT_NAME]" FROM WINDOWS
END


/****** Object:  User [ACCOUNT_NAME]    Script Date: 11/07/2012 12:00:00 PM ******/
IF NOT EXISTS 
	(SELECT * FROM  dbo.sysusers WHERE name = N'[ACCOUNT_NAME]' AND uid < 16382)

	CREATE USER "[ACCOUNT_NAME]"
