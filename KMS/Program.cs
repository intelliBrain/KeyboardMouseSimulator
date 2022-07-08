using System;
using System.Threading;
using System.Threading.Tasks;
using KMS.Extensions;

namespace KMS
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken cancellationToken = tokenSource.Token;

            TimeSpan defaultAutoPauseSpan = TimeSpan.FromMinutes(30);
            TimeSpan autoPauseSpan = defaultAutoPauseSpan;
            DateTime autoPauseTime = DateTime.MaxValue;

            var delaySpan = TimeSpan.FromMilliseconds(200);
            var displayInterval = TimeSpan.FromSeconds(0.5);
            var updateInterval = TimeSpan.FromSeconds(2);
            var nowGlobal = DateTime.Now;
            var nextUpdate = nowGlobal;
            var nextDisplay = nowGlobal;

            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    var kms = new KMSimulator();
                    var rnd = new Random(Environment.TickCount);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var now = DateTime.Now;
                        
                        if (now > nextUpdate)
                        {
                            var outputText = "";
                            var workEnd = new DateTime(now.Year, now.Month, now.Day, 18, 30, 00);
                            var workStart = new DateTime(now.Year, now.Month, now.Day, 07, 00, 00);
                            var dayOfWeek = now.DayOfWeek;
                            var isWeekend = dayOfWeek == DayOfWeek.Sunday || dayOfWeek == DayOfWeek.Sunday;

                            var isPaused = false;

                            if (isWeekend)
                            {
                                isPaused = true;
                                outputText = "UPDATE PAUSED - WEEKEND";
                            }
                            else if (now > workEnd)
                            {
                                isPaused = true;
                                outputText = $"UPDATE PAUSED - after work end: {workEnd:HH:mm}";
                            }
                            else if (now < workStart)
                            {
                                isPaused = true;
                                outputText = $"UPDATE PAUSED - before work start: {workStart:HH:mm}";
                            }
                            else if (now < autoPauseTime)
                            {
                                isPaused = true;
                                outputText = $"UPDATE PAUSED - auto pause: {autoPauseTime:HH:mm}";
                            }

                            if (isPaused)
                            {
                                var newX = rnd.Next(-2, 2);
                                var newY = rnd.Next(-2, 2);

                                kms.MoveDelta(newX, newY);
                                outputText = $"x={newX,4}, y={newY,4}";
                            }

                            Console.SetCursorPosition(0, 0);
                            Console.WriteLine(outputText.EnsureExactLength(70));
                            Console.ResetColor();
                            nextUpdate = now + updateInterval;
                        }


                        if (now > nextDisplay)
                        {
                            var outputText = "";

                            if (autoPauseSpan < TimeSpan.MaxValue)
                            {
                                
                                if (autoPauseTime < DateTime.MaxValue)
                                {
                                    var remaining = autoPauseTime - now;
                                    if (remaining <= TimeSpan.Zero)
                                    {
                                        remaining = TimeSpan.Zero;
                                        nextUpdate = DateTime.Now;
                                    }
                                    Console.ForegroundColor = ConsoleColor.Green;

                                    
                                    if (remaining < TimeSpan.FromMinutes(1))
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                    }
                                    else if (remaining < TimeSpan.FromMinutes(10))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    }

                                    outputText = $"AutoPause: {autoPauseSpan.Hours:00}h  {autoPauseSpan.Minutes:00}m   =>  remaining: {remaining.Hours:00}h  {remaining.Minutes:00}m  {remaining.Seconds:00}s  => {autoPauseTime:HH:mm:ss}";
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    outputText = $"AutoPause: {autoPauseSpan.Hours:00}h  {autoPauseSpan.Minutes:00}m   =>  NOT STARTED (start with [S])";
                                }
                            }

                            Console.SetCursorPosition(0, 2);
                            Console.WriteLine(outputText.EnsureExactLength(70));
                            Console.ResetColor();
                            nextDisplay = now + displayInterval;
                        }

                        Task.Delay(delaySpan, cancellationToken).Wait(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            });

            var running = true;
            var spanMax = TimeSpan.FromHours(12);
            var spanMin = TimeSpan.FromMinutes(1);

            while (running)
            {
                var key = Console.ReadKey(true);
                var now = DateTime.Now;

                var isAlt = ((key.Modifiers & ConsoleModifiers.Alt) != 0);
                var isShift = ((key.Modifiers & ConsoleModifiers.Shift) != 0);
                var isCtrl = ((key.Modifiers & ConsoleModifiers.Control) != 0);
                var anyModifier = isAlt | isShift | isCtrl;

                var ctrlText = isCtrl ? "CTRL" : "    ";
                var shiftText = isShift ? "SHIFT" : "     ";
                var altText = isAlt ? "ALT" : "   ";

                //Console.SetCursorPosition(0, 5);
                //Console.WriteLine($"Modifier: {ctrlText} | {shiftText} | {altText}");


                var spanSkip = (anyModifier) ? TimeSpan.FromMinutes(1) : TimeSpan.FromMinutes(15);
                var spanInit = (anyModifier) ? TimeSpan.FromMinutes(60) : TimeSpan.FromMinutes(15);

                var autoStartSpanBefore = autoPauseSpan;

                switch (key.Key)
                {
                    case ConsoleKey.C:
                        Console.Clear();
                        break;

                    case ConsoleKey.S:
                        if (autoPauseSpan < TimeSpan.MaxValue)
                        {
                            if (autoPauseTime == DateTime.MaxValue)
                                autoPauseTime = now + autoPauseSpan;
                            else
                                autoPauseTime = DateTime.MaxValue;
                        }
                        break;

                    case ConsoleKey.NumPad0:
                        autoPauseSpan = defaultAutoPauseSpan;
                        autoPauseTime = DateTime.MaxValue;
                        break;

                    case ConsoleKey.NumPad1:
                        autoPauseSpan = anyModifier ? TimeSpan.FromMinutes(15) : TimeSpan.FromHours(1);
                        break;

                    case ConsoleKey.NumPad2:
                        autoPauseSpan = anyModifier ? TimeSpan.FromMinutes(30) : TimeSpan.FromHours(2);
                        break;

                    case ConsoleKey.NumPad3:
                        autoPauseSpan = anyModifier ? TimeSpan.FromMinutes(45) : TimeSpan.FromHours(3);
                        break;

                    case ConsoleKey.NumPad4:
                        autoPauseSpan = TimeSpan.FromHours(4);
                        break;

                    case ConsoleKey.NumPad5:
                        autoPauseSpan = TimeSpan.FromHours(5);
                        break;

                    case ConsoleKey.NumPad6:
                        autoPauseSpan = TimeSpan.FromHours(6);
                        break;

                    case ConsoleKey.NumPad7:
                        autoPauseSpan = TimeSpan.FromHours(7);
                        break;

                    case ConsoleKey.NumPad8:
                        autoPauseSpan = TimeSpan.FromHours(8);
                        break;

                    case ConsoleKey.NumPad9:
                        autoPauseSpan = TimeSpan.FromHours(9);
                        break;

                    case ConsoleKey.Add:
                        {
                            if (autoPauseSpan == TimeSpan.MaxValue)
                            {
                                autoPauseSpan = spanInit;
                            }
                            else
                            {
                                autoPauseSpan += spanSkip;
                            }

                            if (autoPauseSpan > spanMax)
                            {
                                autoPauseSpan = spanMax;
                            }
                        }
                        break;

                    case ConsoleKey.Subtract:
                        {
                            if (autoPauseSpan == TimeSpan.MaxValue)
                            {
                                autoPauseSpan = spanInit;
                            }
                            else
                            {
                                autoPauseSpan -= spanSkip;
                            }

                            if (autoPauseSpan < spanMin)
                            {
                                autoPauseSpan = spanMin;
                            }
                        }
                        break;

                    case ConsoleKey.X:
                    case ConsoleKey.Escape:
                        running = false;
                        break;
                }

                if (autoStartSpanBefore != autoPauseSpan)
                {
                    if (autoPauseTime < DateTime.MaxValue)
                    {
                        autoPauseTime = now + autoPauseSpan;
                    }

                    nextDisplay = now;
                }
            }

            tokenSource.Cancel();
            Console.WriteLine($"wait to complete the background task");

            task.Wait();
            Console.WriteLine($"completed");
        }
    }
}
