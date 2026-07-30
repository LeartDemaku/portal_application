USE PortalApp; 
GO


DELETE FROM ArticleArticleTags;
DELETE FROM Articles;
DELETE FROM ArticleCategories;
DELETE FROM ArticleTags;


INSERT INTO [dbo].[ArticleCategories] ([Name], [CreatedAt]) 
VALUES 
('Sport', GETDATE()),
('Siguri', GETDATE()),
('Teknologji', GETDATE()),
('Ekonomi', GETDATE()),
(N'Shëndetësi', GETDATE()),
('Globale', GETDATE()),
(N'Politikë', GETDATE());


INSERT INTO [dbo].[Articles] ([title], [Content], [ArticleCategoryId], [CreatedAt], [CoverImage]) 
VALUES 
(N'Irani po pozicionohet publikisht, por i etur për të negociuar', 
 N'Sekretarja e shtypit e Shtëpisë së Bardhë thotë se regjimi në Iran është gjithnjë e më i etur për të negociuar me SHBA-në ndërsa ende munden dhe se këtë bisedime po shkojnë mirë privatisht.', 
 (SELECT Id FROM ArticleCategories WHERE Name = 'Siguri'), 
 DATEADD(MINUTE, 10, GETDATE()), 
 'lajmi_kryesor.png');


INSERT INTO [dbo].[Articles] ([title], [Content], [ArticleCategoryId], [CreatedAt], [CoverImage]) VALUES 
(N'Stadiumi i Prishtinës', N'Punimet në stadium po ecin sipas planit të paraparë.', (SELECT Id FROM ArticleCategories WHERE Name = 'Sport'), GETDATE(), '1.png'),
(N'Ndeshja e Kosovës', N'Kombëtarja përgatitet për sfidën e radhës në udhëtim.', (SELECT Id FROM ArticleCategories WHERE Name = 'Sport'), GETDATE(), '2.jpg'),
(N'Transferimet e Reja', N'Shumë lëvizje priten në afatin kalimtar të verës.', (SELECT Id FROM ArticleCategories WHERE Name = 'Sport'), GETDATE(), '3.jpg'),
(N'OpenAI po mbyll aplikacionin Sora', N'Kompania OpenAI ka konfirmuar se po e mbyll aplikacionin e saj për gjenerimin e videove me emrin Sora.', (SELECT Id FROM ArticleCategories WHERE Name = 'Teknologji'), GETDATE(), '4.jpg'),
(N'Lufta në Iran, aeroplanmbajtësja e tretë', N'Marinsat amerikanë mbërritën në ujërat e Komandës Qendrore të SHBA-së të premten.', (SELECT Id FROM ArticleCategories WHERE Name = 'Siguri'), GETDATE(), '5.jpg'),
(N'GAP: Kosova mbi mesataren në transparencë', N'Kosova ka shënuar progres në transparencën buxhetore, duke u renditur mbi mesataren e rajonit.', (SELECT Id FROM ArticleCategories WHERE Name = 'Ekonomi'), GETDATE(), '6.png'),
(N'Robotët po shkojnë drejt luftës', N'Startup-i amerikan Foundation Robotics thuhet se ka dërguar robotët humanoidë në Ukrainë.', (SELECT Id FROM ArticleCategories WHERE Name = 'Teknologji'), GETDATE(), '7.png'),
(N'Meta paguan 375 milionë dollarë gjobë', N'Një gjykatë në New Mexico ka urdhëruar Meta-n të paguajë gjobën për mashtrimin e përdoruesve.', (SELECT Id FROM ArticleCategories WHERE Name = 'Teknologji'), GETDATE(), '8.png');



IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Teknologji') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Teknologji', GETDATE());
IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Sport') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Sport', GETDATE());
IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Siguri') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Siguri', GETDATE());
IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Ekonomi') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Ekonomi', GETDATE());
IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Robotikë') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Robotikë', GETDATE());
IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Shëndetësi') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Shëndetësi', GETDATE());
IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Globale') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Globale', GETDATE());
IF NOT EXISTS (SELECT * FROM ArticleTags WHERE Title = N'Politikë') 
    INSERT INTO [dbo].[ArticleTags] ([Title], [CreatedAt]) VALUES (N'Politikë', GETDATE());



INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES ((SELECT Id FROM Articles WHERE title = N'Irani po pozicionohet publikisht, por i etur për të negociuar'), (SELECT Id FROM ArticleTags WHERE Title = N'Siguri'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES ((SELECT Id FROM Articles WHERE title = N'Stadiumi i Prishtinës'), (SELECT Id FROM ArticleTags WHERE Title = N'Sport'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES ((SELECT Id FROM Articles WHERE title = N'Ndeshja e Kosovës'), (SELECT Id FROM ArticleTags WHERE Title = N'Sport'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES ((SELECT Id FROM Articles WHERE title = N'Transferimet e Reja'), (SELECT Id FROM ArticleTags WHERE Title = N'Sport'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES ((SELECT Id FROM Articles WHERE title LIKE N'OpenAI%'), (SELECT Id FROM ArticleTags WHERE Title = N'Teknologji'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES ((SELECT Id FROM Articles WHERE title LIKE N'Lufta në Iran%'), (SELECT Id FROM ArticleTags WHERE Title = N'Siguri'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES ((SELECT Id FROM Articles WHERE title LIKE N'GAP:%'), (SELECT Id FROM ArticleTags WHERE Title = N'Ekonomi'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES 
((SELECT Id FROM Articles WHERE title LIKE N'Robotët%'), (SELECT Id FROM ArticleTags WHERE Title = N'Robotikë'), GETDATE()),
((SELECT Id FROM Articles WHERE title LIKE N'Robotët%'), (SELECT Id FROM ArticleTags WHERE Title = N'Teknologji'), GETDATE());


INSERT INTO [dbo].[ArticleArticleTags] ([ArticleId], [ArticleTagId], [CreatedAt])
VALUES 
((SELECT Id FROM Articles WHERE title LIKE N'Meta%'), (SELECT Id FROM ArticleTags WHERE Title = N'Siguri'), GETDATE()),
((SELECT Id FROM Articles WHERE title LIKE N'Meta%'), (SELECT Id FROM ArticleTags WHERE Title = N'Teknologji'), GETDATE());


SELECT a.title AS Artikulli, t.Title AS Tagu
FROM Articles a
JOIN ArticleArticleTags at ON a.Id = at.ArticleId
JOIN ArticleTags t ON at.ArticleTagId = t.Id;
