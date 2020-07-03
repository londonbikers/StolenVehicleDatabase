ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [SVD_log], FILENAME = 'C:\temp2\Code\db\SVD_log.ldf', SIZE = 1024 KB, MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);





