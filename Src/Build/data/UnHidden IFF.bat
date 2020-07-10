@echo off
echo -------------------------------------------------------
echo -      This script unhide all iff in same folder      -
echo -------------------------------------------------------

attrib -S -H "*.iff"
attrib -S -H "*.sff"
echo -------------------------------------------------------
pause