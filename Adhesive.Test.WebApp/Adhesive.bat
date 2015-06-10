@echo on
ilmerge /ndebug /target:dll /targetplatform:v2 /out:Adhesive.dll /log /xmldocs Adhesive.AppInfoCenter.dll Adhesive.AppInfoCenter.Imp.dll Adhesive.Common.dll Adhesive.Config.dll Adhesive.Config.Imp.dll Adhesive.MemoryQueue.dll Adhesive.MemoryQueue.Imp.dll Adhesive.Mongodb.dll Adhesive.Mongodb.Imp.dll Adhesive.Mongodb.Server.dll
pause