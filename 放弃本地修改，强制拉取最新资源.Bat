echo. ==================
echo. 按回车重置工程，若放弃，则关闭窗口即可
echo. ==================
pause

git fetch --all
git reset --hard origin
git pull --progress "origin"
pause