SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		������� ����
-- Create date: 03.04.2023
-- Description:	��������� ���������� ������� �����
/*
Declare @PLastDateReflection datetime = convert(datetime,convert(varchar, '2023-04-03 23:35:59.872', 21),21);
exec [dbo].[PulBasicSetStatus] @MachineName='ISV', @CustomClassType='ProviderPul', @LastDateReflection=@PLastDateReflection, @VersionPul='1.0.0.0', @LastStatusCustom='Success'
*/
-- =============================================
ALTER PROCEDURE [dbo].[PulBasicSetStatus]		
	@MachineName varchar(100),
	@CustomClassTyp varchar(300),
	@LastDateReflection datetime,
	@VersionPul varchar(50),
	@LastStatusCustom varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	Declare�@ErrorMessage�varchar(1000),�@ErrorMes�nvarchar(max),�@CurrentDateTime�DateTime=GETDATE();
    
	--�
� � Declare @CurNameProcedure varchar(100) = '[dbo].[PulBasicSetStatus]';
    Print�'����������� � ������ �������: '+@CurNameProcedure;�
� � --Declare�@LogGUID�table�(LogGUID�uniqueidentifier);�
� � --Insert�[ddl].[CreateMirror_Log]�([DateTime],RealStart,Script)�
� � --Output�inserted.[GUID]�into�@LogGUID(LogGUID)�
� � --Values(@CurrentDateTime,GETDATE(),'[ddl].[CreateMirror_Log] @database='+coalesce(@database,'null')�+�', @HistoryLogDay='+coalesce(Convert(nvarchar,@HistoryLogDay),'null'));�

	-- � � � ��
� � begin�try�
� � ----------------------�
� � � � ------------------------------------------------�
� � � � Set�@ErrorMessage='	1. �������� �� ������������� ������ ����.';�Print�@ErrorMessage;�� � � � � � � ��
� � � � -- � � � �Declare @ErrorMes nvarchar(max), @database varchar(100)='dr201602';�
� �-- � � � ��if�not�exists�(select�1�From�sys.databases�Where�name=@database)�
� � �-- � � ��begin�
� � � �-- � � � ��Set�@ErrorMes='���� � ������: '�+�@database�+�' �� ����������.';�
� � � � �-- � � ��RaisError(@ErrorMes,12,1);�� � � ��
� � � � � --��end�
			if exists (Select Top 1 1 
					From [dbo].[PulBasicStatus] 
					Where [MachineName] = @MachineName
						and [CustomClassTyp] = @CustomClassTyp)
				begin
					Print '���������� ������' 
					Update [dbo].[PulBasicStatus]
					Set [LastDateReflection]=@LastDateReflection, [VersionPul]=@VersionPul, [LastStatucCustom]=@LastStatusCustom
					Where [MachineName] = @MachineName
						and [CustomClassTyp] = @CustomClassTyp;
				end
			else 
				begin
					Print '������� ����� ������'
					insert into [dbo].[PulBasicStatus]([MachineName], [CustomClassTyp], [LastDateReflection], [VersionPul], [LastStatucCustom])
					Values (@MachineName, @CustomClassTyp, @LastDateReflection, @VersionPul, @LastStatusCustom);
				end

� � � � ------------------------------------------------�
� � � � Set�@ErrorMessage=' ������������ � �������.';�Print�@ErrorMessage;�

            Insert into [dbo].[Log] ([DateTime], [Message], [Source], [Status])
			Values(GetDate(), @ErrorMessage, @CurNameProcedure, 'Message');


� � � � � ��--Update�[ddl].[CreateMirror_Log]�Set�RealEnd=GETDATE()�Where�[GUID]=(Select�LogGUID�from�@LogGUID);�
� � ----------------------�
� � end�try�
� ��begin�catch�
� �     if�ERROR_NUMBER()=50000�
� � � � � ��Begin�
     �  � � � ��set�@ErrorMessage=ERROR_MESSAGE();�
� � � � � ��End�
� � � � Else�
� � � � � ��Begin�
� �  � �� � � ��set�@ErrorMessage='��������� ������ �� ����: '+�@ErrorMessage+' ('+Convert(nvarchar,ERROR_NUMBER())�+�' - '+�ERROR_MESSAGE()�+�')';�
� � � � � ��End�
� � � ��--�
� � � � Print�'�������� ������'�
� � � � Set�@ErrorMes=Convert(varchar(15),ERROR_NUMBER())+' - '+Replace(@ErrorMessage,'''','''''');�
� � � ��--�
� � � ��--Update�[ddl].[CreateMirror_Log]�Set�RealEnd=GETDATE(),�Error=@ErrorMes�
� � � ��--Where�[GUID]=(Select�LogGUID�from�@LogGUID);�
� � � ��--�
		Insert into [dbo].[Log] ([DateTime], [Message], [Source], [Status])
		Values(GetDate(), @ErrorMes, @CurNameProcedure, 'Error');
		--
� � � ��raiserror(@ErrorMes,12,1);�
� � end�catch�� � � � � � � ��


END
GO
