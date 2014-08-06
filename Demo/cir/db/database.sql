SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[cir_case_state]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[cir_case_state](
	[id] [int] NOT NULL,
	[stateName] [nvarchar](50) NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[cir_case_images]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[cir_case_images](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[caseId] [int] NULL,
	[imgUrl] [nvarchar](150) NOT NULL,
	[createTime] [datetime] NOT NULL,
	[imgType] [int] NOT NULL CONSTRAINT [DF__cir_case___imgTy__014935CB]  DEFAULT ((1)),
 CONSTRAINT [PK__cir_case_images__00551192] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[cir_case]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[cir_case](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cashNo] [nvarchar](100) NOT NULL,
	[partnerCode] [nvarchar](100) NOT NULL,
	[contractNo] [nvarchar](100) NOT NULL,
	[createTime] [datetime] NOT NULL,
	[importTime] [datetime] NOT NULL,
	[state] [int] NOT NULL,
	[cusCaseNo] [nvarchar](100) NOT NULL,
	[caseNo] [nvarchar](100) NOT NULL,
	[province] [nvarchar](100) NULL,
	[city] [nvarchar](100) NULL,
	[service] [nvarchar](300) NULL,
	[cw] [nvarchar](50) NULL,
	[bx] [nvarchar](50) NULL,
	[personName] [nvarchar](50) NULL,
	[contract] [nvarchar](50) NULL,
 CONSTRAINT [PK__cir_case__7C8480AE] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
