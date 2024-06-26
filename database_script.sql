USE [SkelbimuSistemosDb]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2024-05-13 12:18:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 2024-05-13 12:18:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Price] [float] NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[StartDate] [nvarchar](max) NOT NULL,
	[EndDate] [nvarchar](max) NOT NULL,
	[UserId] [int] NOT NULL,
	[Category] [int] NOT NULL,
	[PaymentType] [int] NOT NULL,
	[State] [int] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reports]    Script Date: 2024-05-13 12:18:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Reason] [nvarchar](max) NOT NULL,
	[UserId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suspensions]    Script Date: 2024-05-13 12:18:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suspensions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Reason] [nvarchar](max) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Corrected] [bit] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Reviewed] [bit] NOT NULL,
 CONSTRAINT [PK_Suspensions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2024-05-13 12:18:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[FirstName] [nvarchar](max) NOT NULL,
	[LastName] [nvarchar](max) NOT NULL,
	[PasswordHash] [varbinary](max) NOT NULL,
	[PasswordSalt] [varbinary](max) NOT NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[Role] [int] NOT NULL,
	[Blocked] [bit] NOT NULL,
	[VerificationToken] [nvarchar](max) NULL,
	[VerificationDate] [datetime2](7) NULL,
	[PasswordResetToken] [nvarchar](max) NULL,
	[ResetTokenExpirationDate] [datetime2](7) NULL,
	[SearchKeyWords] [nvarchar](max) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserWishes]    Script Date: 2024-05-13 12:18:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserWishes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[SearchKeyWords] [nvarchar](max) NOT NULL,
	[PaymentMethod] [int] NOT NULL,
	[PriceHigh] [float] NOT NULL,
	[PriceLow] [float] NOT NULL,
	[Category] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_UserWishes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316104046_Initial', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316170802_Test1', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316171423_AddReports', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240316203302_AddSuspensions', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240410012156_ProductUpdatee', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240417152753_ProductStateAndPaymentType', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240417160309_AddSuspensionsForReal', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240418104642_ModelPropertyChanges', N'8.0.4')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20240501133703_AddedUserWishesTable', N'8.0.4')
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (1, N'AOC monitor', N'good monitor', 20, N'https://skelbiu-img.dgn.lt/1_23_3864438925/aoc-24g2u-bk-144hz-ips.jpg', N'2024-05-13', N'2024-06-13', 1, 8, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (2, N'corsair klaviatura', N'ssviecia', 10, N'https://images.kaina24.lt/3130/87/corsair-strafe-mk-2.jpg', N'2024-05-13', N'2024-06-13', 2, 8, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (3, N'Amd', N'Naujas', 30, N'https://ksd-images.lt/display/aikido/cache/08983d0360b822b8cdcef8773b30db29.jpeg', N'2024-05-13', N'2024-06-13', 2, 1, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (4, N'Korpusas DEEPCOOL', N'Pirktas naujas, bet buvo per mažas. Yra garantija', 60, N'https://www.varle.lt/static/uploads/products/572/dee/deepcool-case-ch510-side-window-juodas-1fb5a5ba3f.png', N'2024-05-13', N'2024-06-13', 2, 6, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (5, N'Intel i5', N'10-kartos procesoriukas, nedaug sukes, neoverclock''intas, gero stovio.', 45, N'https://thumbor.arvutitark.ee/17NYdAYYuxY1M7nxz4nKJbCzxLQ=/trim/fit-in/800x800/filters:format(webp)/https%3A%2F%2Fstatic.arvutitark.ee%2Fpublic%2Fmedia-hub-olev%2F2023%2F02%2F349914%2Fhpit-619_hpit_619_01.jpg', N'2024-05-13', N'2024-06-13', 1, 1, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (6, N'Asus GeForce RTX 2060', N'Parduodama vaizdo plokštė Asus GeForce RTX 2060. 6GB, Memory Type GDDR6', 195, N'https://i.ebayimg.com/images/g/~lQAAOSw3bNj1gwe/s-l1200.webp', N'2024-05-13', N'2024-05-21', 1, 2, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (7, N'Gigabyte B760 GAMING X AX', N'Pagrindinė plokštė Gigabyte B760 GAMING X AX DDR4 (rev. 1.0), ATX, LGA1700, DDR4, WiFi. Parduodu, nes netiko mano korpusui. Naujos kaina 159.', 110, N'https://lt2.pigugroup.eu/colours/673/553/61/67355361/gigabyte-b760-gaming-x-ax-ddr4-rev-099e7_reference.jpg', N'2024-05-13', N'2024-05-16', 1, 0, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (8, N'Crucial 32GB (2x 16GB) DDR5 5600MHz', N'Parduodama nauja atmintis, turime daug vienetų, rašykite, kaina paderinsim.', 85, N'https://cdn.mos.cms.futurecdn.net/jhPWkBxKU4iNdue38zxH2K.jpg', N'2024-05-13', N'2024-06-05', 3, 3, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (9, N'Kietasis diskas (SSD) Samsung 980 PRO', N'Labai rekomenduoju tam, kas nori pagreitinti kompiuterio greiti, pats toki naudoju labai patenkintas.', 83, N'https://ksd-images.lt/display/aikido/store/ebf6ec652ed6bc4b81b9a1d8a2c77a08.jpg', N'2024-05-13', N'2024-05-21', 3, 4, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (10, N'Maitinimo blokas MSI', N'Parduodu nauja MSI MPG A750GF 750 W, 14 cm. žaidimų maitinimo blokas gali palaikyti NVIDIA GeForce RTX™ 30/20 serijos ir AMD GPU. MPG maitinimo bloko IO atramos, parengtos aukščiausiems reikalavimams, gali palaikyti skirtingus ir universalius prijungimo būdus, atsižvelgiant į skirtingų vaizdo plokščių maitinimo jungčių dizainą', 91, N'https://ksd-images.lt/display/aikido/store/ee3d04a24bc46153ec6b904deb7ac350.jpg?h=742&w=816', N'2024-05-13', N'2024-06-02', 2, 5, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (11, N'Ausinimo ventiliatorius', N'CPU Ausintuvas AMD RYZEN Wraith STEALTH AM4 Naudotas. Veikia Gerai. Kaina 12 EUR.', 12, N'https://skelbiu-img.dgn.lt/1_22_3358389944/cpu-cooler-intel-1200-1151-1155-1150-ryzen-am4.jpg', N'2024-05-13', N'2024-06-01', 1, 7, 0, 0)
INSERT [dbo].[Products] ([Id], [Name], [Description], [Price], [ImageUrl], [StartDate], [EndDate], [UserId], [Category], [PaymentType], [State]) VALUES (12, N'Parduodamas Playsation 4', N'Parduodu mazai naudota, geros bukles ir puikiai veikiancia PS 4 Pro 1GB zaidimu konsole. I komplekta ieina pati konsole, kontroleris ir visi reikalingi laidai. Tiek konsole tiek kontroleris veikia be priekaistu.', 170, N'https://skelbiu-img.dgn.lt/1_22_3953497048/parduodu-naudota-playstation-4-pro-1gb-konsole.jpg', N'2024-05-13', N'2024-06-09', 3, 9, 0, 0)
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([Id], [Email], [FirstName], [LastName], [PasswordHash], [PasswordSalt], [PhoneNumber], [Role], [Blocked], [VerificationToken], [VerificationDate], [PasswordResetToken], [ResetTokenExpirationDate], [SearchKeyWords]) VALUES (1, N'pamatas8@gmail.com', N'matas1', N'matas1', 0x66D1BB3E9EE6883A17CF2079147FB33208BF6D9F9072D6B4B758AA3E815980B98DCFD4E109B551D828092855C09B6F928FD6345756821CAA27D9FED421738FB2, 0x8BF7E257D03F68282CD337646FD699602906A7CF0D480EE2CAA0C1A911A73F488EA698C0E0834F63331839CD9184687DB2D325A57CD29700EC6F0312E857E83501D5E9EE23EA6CEEA6A5D83B7C9173CB7038F47724FF88EAA63225E23A6F6A1E3C17B17A5CFFDB2E8EC19855EE0E59BB7659819ED5EA8FF3FD312871DCBEE4DD, N'863871234', 2, 0, N'419D2BBFD3AAA2F83EA11A01F05132B15DFE9AED76B16C1890AA0EF9C928F79757ADB5205D392BD363529850ED7A56EE012D8800A3D89B51443301CDE1A8404F', CAST(N'2024-05-12T17:35:43.9121953' AS DateTime2), NULL, NULL, N'aoc,aoc,aoc,aoc,aoc,')
INSERT [dbo].[Users] ([Id], [Email], [FirstName], [LastName], [PasswordHash], [PasswordSalt], [PhoneNumber], [Role], [Blocked], [VerificationToken], [VerificationDate], [PasswordResetToken], [ResetTokenExpirationDate], [SearchKeyWords]) VALUES (2, N'vicilay942@facais.com', N'vicilay', N'vicilay', 0xFB044BF8911119F0C689CC32AC81983F59EC5CC67D7BBE74C61108486B6ED79B316E3A9E1686DE0640AA1F52040EB6B06AB52A8713608F834690E0976A12FD14, 0xC71893A9FC4A52435F1C5B265288C333349A35DD71383FD6C60A3EF8F39B56D85095BFC899E20D2F58B2089F581A9983169208A385C60746DCF72039952C8B70DD17F4FA13847F93F09150130BB7D88567F8659F5592239210D2E22618439F6875379C448C3AE160EBAB6192117090A9BA73526688A5F40C40D919597E41A209, N'868750062', 2, 0, N'0E57E932C23D015BCACF23321C787CA2B0D960201637823019D83D696DBAC4A7AA9C1754D03C3D8CF181779E7C40C68B018F51FBDAE90585FFDDC573E885517A', CAST(N'2024-05-13T11:11:43.9000000' AS DateTime2), NULL, NULL, N'')
INSERT [dbo].[Users] ([Id], [Email], [FirstName], [LastName], [PasswordHash], [PasswordSalt], [PhoneNumber], [Role], [Blocked], [VerificationToken], [VerificationDate], [PasswordResetToken], [ResetTokenExpirationDate], [SearchKeyWords]) VALUES (3, N'admin@gmail.com', N'admin', N'admin', 0xBA94F4DF9B270158B24884E762EE11B4CA2939BB8FAEE37871C2625BA5E54A180AB1C42F67E1A5D42F3649F61B472AB7A3267077AE535733BE9C68EB42EAA1D3, 0xE0FBF01ECBF7210017E27B0DD48A8511C6A346B1C3F0CC7F0E1E0E24E87980657679BB137D61D42C9423BD271335A5EC7B00FF5A7D9D1F0FD9BABACEB53B10B118F2D890AE005B10778A1ECFA49515184882BA0C99A55888D2D49C307C6BB261931E55035C3B6132557BB9DEDAA7EC44246743CA7A6D6CA402962D556B6CA39E, N'+37081346785', 3, 0, N'CC8C20B65E762D43891C6C1AC6CAFBBE3ECDCCDE417BD604A39AC6ABC98F7AA9AD3D87C255650052564CB47CA4C93050B35CA99C2B09FA1A027B630466DCE48A', CAST(N'2024-05-13T11:26:43.9000000' AS DateTime2), NULL, NULL, N'aoc,s,')
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [PaymentType]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [State]
GO
ALTER TABLE [dbo].[Suspensions] ADD  DEFAULT (CONVERT([bit],(0))) FOR [Reviewed]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Users_UserId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Products_ProductId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Users_UserId]
GO
ALTER TABLE [dbo].[Suspensions]  WITH CHECK ADD  CONSTRAINT [FK_Suspensions_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Suspensions] CHECK CONSTRAINT [FK_Suspensions_Products_ProductId]
GO
ALTER TABLE [dbo].[UserWishes]  WITH CHECK ADD  CONSTRAINT [FK_UserWishes_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserWishes] CHECK CONSTRAINT [FK_UserWishes_Users_UserId]
GO
