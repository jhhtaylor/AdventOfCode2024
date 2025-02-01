

## To generate frames

After running the frame generation with `dotnet run` and then choosing `601`, the frames will be generated in `Day6Frames` folder.`

## To generate video
Run `ffmpeg -framerate 10 -i Day6Frames/frame_%04d.png -c:v libx264 -r 30 -pix_fmt yuv420p Day6Outputs/output.mp4`