@echo off
scp roleplay.zip server@51.158.125.63:/home/server/ragemp-srv/roleplay.zip
ssh server@51.158.125.63 cd /home/server/ragemp-srv && '/home/server/ragemp-srv/unpack.sh'