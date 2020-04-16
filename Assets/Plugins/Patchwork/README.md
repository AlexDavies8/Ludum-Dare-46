# Patchwork
Patchwork is a node-based texture editing and creating tool, focused on pixel art adjustment and automatic tileset generation.
There are many nodes available, and node graphs can be saved out as JSON files. Images can also be exported from the tool as PNGs.

# Nodes
Here is a summary of the current availiable selection of nodes
## Input
### Image Texture
```
  Inputs:
    "Texture", Texture2D (Field)
  Outputs:
    "Out", Texture (Port)
    
  Passes "Texture" to "Out".
```

### Colour Texture
```
  Inputs: 
    "Colour", Colour (Field)
  Outputs: 
    "Out", Texture (Port)
    
  Passes 4x4 texture to "Out", with every pixel set to "Colour".
```
  
### Circle Texture
```
  Inputs: 
    "Size", Integer 1 to 128 (Field)
  Outputs: 
    "Out", Texture (Port)
    
  Passes white circular texture to "Out", with dimensions "Size" by "Size".
```
  
## Adjust
### HSV Adjust
```
  Inputs: 
    "In", Texture2D (Port)
    "Hue", Integer -180 to 180 (Field)
    "Saturation", Integer 0 to 100 (Field)
    "Value", Integer 0 to 100 (Field)
  Outputs: 
    "Out", Texture (Port)
    
  Each pixel in "In":
    1. Is Hue shifted by "Hue" degrees,
    2. Has it's saturation multiplied by 1 + "Saturation" / 100,
    3. Has it's value multiplied by 1 + "Value" / 100
  Passes the resulting Texture2D to "Out".
```

### Offset
```
  Inputs:
    "In", Texture2D (Port)
    "X Offset", Integer 0 to 100 (Field)
    "Y Offset", Integer 0 to 100 (Field)
  Outputs:
    "Out", Texture2D (Port)
  
  Passes "In", shifted on the x axis by "X Offset", and on the y axis by "Y Offset", to "Out"
  It is shifted by a percentage of "In"s width/height, and wraps.
```

###Resample
```
  Inputs:
    "In", Texture2D (Port)
    "X Size", Integer 1 to 128 (Field)
    "Y Size", Integer 1 to 128 (Field)
  Outputs:
    "Out", Texture2D (Port)
    
  Passes "In" to "Out", resampled to dimensions "X Size" by "Y Size" using nearest neighbour sampling.
  NOTE: This node doesn't work correctly at the moment, and will also 'squash' the image by one pixel
```

### Smart Downscale
```
  Inputs:
    "In", Texture2D (Port)
    "Downscale Multiplier", Integer 1 to 8 (Field)
  Outputs:
    "Out", Texture2D (Port)
    
  Passes "In" to "Out", downscaled by a factor of "Downscale Multiplier", using the following method:
    1. Split the texture into blocks of width and height "Downscale Multiplier",
    2. Average the colour values of the pixels in the block
    3. Find the closest colour to the average colour within the block
  This method is more consistant, and results in less aliasing on larger textures than Resample, but can lose fine details.
```

### Erode
```
  Inputs:
    "In", Texture2D (Port)
  Outputs:
    "Out", Texture2D (Port)
  
  For each pixel in "In", if any cardinally neighbouring pixel has an alpha value of 0, sets the pixel's alpha value to 0.
  Passes the resulting Texture2D to "Out".
  
  NOTE: This node is incomplete, and 
```
