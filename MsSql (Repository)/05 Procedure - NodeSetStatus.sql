SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ïîãîäèí Èëüÿ
-- Create date: 03.04.2023
-- Description:	Ïğîöåäóğà îáíîâëåíèÿ ñòàòóñà ïóëîâ
/*
Declare @PLastDateReflection datetime = convert(datetime,convert(varchar, '2023-04-03 23:35:59.872', 21),21);
exec [dbo].[NodeSetStatus] @MachineName='ISV', @LastDateReflection=@PLastDateReflection, @VersionNode='1.0.0.0', @LastStatusNode='Success'
*/
-- =============================================
Alter PROCEDURE [dbo].[NodeSetStatus]		
	@MachineName varchar(100),
	@LastDateReflection datetime,
	@VersionNode varchar(50),
	@LastStatusNode varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @ErrorMessage varchar(1000), @ErrorMes nvarchar(max), @CurrentDateTime DateTime=GETDATE();
    
	-- 
    Declare @CurNameProcedure varchar(100) = '[dbo].[NodeSetStatus]';
    Print 'Ğåãèñòğàöèÿ è çàïóñê ñêğèïòà: '+@CurNameProcedure; 
    --Declare @LogGUID table (LogGUID uniqueidentifier); 
    --Insert [ddl].[CreateMirror_Log] ([DateTime],RealStart,Script) 
    --Output inserted.[GUID] into @LogGUID(LogGUID) 
    --Values(@CurrentDateTime,GETDATE(),'[ddl].[CreateMirror_Log] @database='+coalesce(@database,'null') + ', @HistoryLogDay='+coalesce(Convert(nvarchar,@HistoryLogDay),'null')); 

	--         
    begin try 
    ---------------------- 
        ------------------------------------------------ 
        Set @ErrorMessage='	1. Ïğîâåğêà íà ñóùåñòâîâàíèå äàííîé áàçû.'; Print @ErrorMessage;                 
        --        Declare @ErrorMes nvarchar(max), @database varchar(100)='dr201602'; 
   --         if not exists (select 1 From sys.databases Where name=@database) 
     --       begin 
       --         Set @ErrorMes='Áàçû ñ èìåíåì: ' + @database + ' íå ñóùåñòâóåò.'; 
         --       RaisError(@ErrorMes,12,1);         
          --  end 
			if exists (Select Top 1 1 
					From [dbo].[NodeStatus] 
					Where [MachineName] = @MachineName)
				begin
					Print 'Îáíîâëåíèå ñòğîêè' 
					Update [dbo].[NodeStatus]
					Set [LastDateReflection]=@LastDateReflection, [VersionNode]=@VersionNode, [LastStatucNode]=@LastStatusNode
					Where [MachineName] = @MachineName;
				end
			else 
				begin
					Print 'Âñòàâêà íîâîé ñòğîêè'
					insert into [dbo].[NodeStatus]([MachineName], [LastDateReflection], [VersionNode], [LastStatucNode])
					Values (@MachineName, @LastDateReflection, @VersionNode, @LastStatusNode);
				end

        ------------------------------------------------ 
        Set @ErrorMessage='  Çàâåğøèëîñü ñ óñïåõîì.'; Print @ErrorMessage; 

            Insert into [dbo].[Log] ([DateTime], [Message], [Source], [Status])
			Values(GetDate(), @ErrorMessage, @CurNameProcedure, 'Message');


            --Update [ddl].[CreateMirror_Log] Set RealEnd=GETDATE() Where [GUID]=(Select LogGUID from @LogGUID); 
    ---------------------- 
    end try 
    begin catch 
        if ERROR_NUMBER()=50000 
            Begin 
                set @ErrorMessage=ERROR_MESSAGE(); 
            End 
        Else 
            Begin 
                set @ErrorMessage='ÏĞÎÈÇÎØËÀ ÎØÈÁÊÀ ÍÀ ØÀÃÅ: '+ @ErrorMessage+' ('+Convert(nvarchar,ERROR_NUMBER()) + ' - '+ ERROR_MESSAGE() + ')'; 
            End 
        -- 
        Print 'Ôèêñàöèÿ îøèáêè' 
        Set @ErrorMes=Convert(varchar(15),ERROR_NUMBER())+' - '+Replace(@ErrorMessage,'''',''''''); 
        -- 
        --Update [ddl].[CreateMirror_Log] Set RealEnd=GETDATE(), Error=@ErrorMes 
        --Where [GUID]=(Select LogGUID from @LogGUID); 
        -- 
		Insert into [dbo].[Log] ([DateTime], [Message], [Source], [Status])
		Values(GetDate(), @ErrorMes, @CurNameProcedure, 'Error');
		--
        raiserror(@ErrorMes,12,1); 
    end catch                 


END
GO
