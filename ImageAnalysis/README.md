# This folder contains complete image analyser and server

## To set this up you will need to do the following:
- Install python3 on your machine (if you are on Windows just use Anaconda (you will need `opencv` and `imutils` for this to work))
- If you are on ubuntu you can just run `chmod +x ./setup.h` and it shuld install all missing packadges


## Runing this script

`AnalyseImage.py` script can be run with the following arguments:
- `-i`, `--input` - that specifies path to input image file
- `-o`, `--output` - that specifies path to output image file **Note:** if path contains dirrectories that do not exist they wont be created
- `-r`, `--remove` - if this argument is present the extra wgitespace will be removed
- `-a <float>`, `--aproximation <float>` - aproximation factor thats passed in `approxPolyDP` default value is `4` this might help you with precision of shape detection

Basic usage ```python3 AnalyseImage.py -i <InputImage>``` you should be presentet with `test-output.jpg` and `test-output.jpg.json` file