﻿INSERT INTO [dbo].[GameServerGroup] ([Id], [PrivateKey], [PublicKey], [IsActive], [IsValid]) VALUES (N'6ca05184-a7ba-44d9-a523-5f081e51d7b8', N'66edfde5-54d6-4a4d-91b6-40209eb9414c', N'8817992b-34c5-4bfe-89b6-7b074583af26', 1, 1)
INSERT INTO [dbo].[GameServer] ([Id], [PrivateKey], [PublicKey], [GroupId], [Name], [IsActive], [IsValid], [LastActive]) VALUES (N'd01fe450-4de9-46fa-955c-3c23d4d3d4ac', N'4b12123c-896c-4e01-b966-a2cf57b63357', N'1c3976c9-a712-48c9-b69e-d26ef3de0a19', N'6ca05184-a7ba-44d9-a523-5f081e51d7b8', N'Pilssi L4D Stats API development', 1, 1, N'2018-07-05 14:00:41')