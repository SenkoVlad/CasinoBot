IF(NOT EXISTS(select fk.name
              from sys.indexes AS fk
              where fk.name = 'WithdrawRequests_isAccounted_index'
))
BEGIN
    create index WithdrawRequests_isAccounted_index
	on WithdrawRequests (isAccounted)
END
GO