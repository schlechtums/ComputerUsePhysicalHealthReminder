# How To Use
Download the latest [release zip](https://github.com/schlechtums/ComputerUsePhysicalHealthReminder/releases) and run CUPHR.exe.  Timers automatically start as soon as the program is running.  When a timer expires you will get a Windows Toast notification and the choice to start or skip the timer's supplemental action when applicable.  The app's title and taskbar icon will also show a progress for the next expiring timer.  The taskbar icon will change from green, to yellow, to red as the next expiring timer gets closer to expiration.



# Customizing Timers
Timers can be customized by modifying the file [timers.csv](/src/CUPHR.ViewModel/timers.csv)

| Name              | Interval | ActionTime | ElapsedMessages                                | Icon                          |
| ----------------- | -------- | ---------- | ---------------------------------------------- | ----------------------------- |
| Eyes 20-20-20     | 00:20:00 | 00:00:20   | Focus on something 20 feet away for 20 seconds | TimerResources\eye.png        |
| Walking/Stretches | 01:00:00 | 00:05:00   | Walk around or do some stretches               | TimerResources\stretching.png |
| Sit/Stand         | 01:00:00 |            | Stand\|Sit                                     | TimerResources\sitstand.png   |

* Name - The name of the timer
* Interval - How often the timer goes off.  Must be in the format HH:mm:ss (two digit hours, minutes, seconds with a colon separating each)
* Action Time - If the timer should provide a supplemental action, ex. "Focus on something 20 feet away for 20 seconds", then this is the amount of time it should run for.  Same format as the Interval
* ElapsedMessages - The message(s) to show in the windows notification when the timer runs out.  Multiple alternating messages can be defined by using | as a delimiter
* Icon - Optionally define an icon to show for the timer.  The icon must be in a path relative to CUPHR.exe.  You can have an alternate icon to show for the windows notification by providing a second icon file whose name ends with \_toast.  Ex: [stretching.png](/src/CUPHR.ViewModel/TimerResources/stretching.png) and [stretching_toast.png](/src/CUPHR.ViewModel/TimerResources/stretching_toast.png)



## Icons
<div>Icons made by <a href="https://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
<div>Icon made by <a href="https://www.flaticon.com/authors/pixel-perfect" title="Pixel perfect">Pixel perfect</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
