namespace BaraTest

module MainActivity =

  open System
  open Android.App
  open Android.Content
  open Android.OS
  open Android.Runtime
  open Android.Views
  open Android.Widget
  open Android.Hardware

  [<Activity (Label = "HelloBarometer", MainLauncher = true)>]
  type MainActivity () =
    inherit Activity ()
    
    let calculateAltitudeInFeet hPAs =
        let pstd = 1013.25
        (1.0 - Math.Pow((hPAs/pstd), 0.190284)) * 145366.45
        
    let mainLabel = ref Unchecked.defaultof<_>

    override x.OnCreate(bundle) =
        base.OnCreate(bundle)
        //Set our view from the "main" layout resource
        x.SetContentView(Resource_Layout.Main)
        
        //Detect the barometer
        let sm = x.GetSystemService(Context.SensorService) :?> SensorManager
        let pressure = sm.GetDefaultSensor(SensorType.Pressure)
                             
        //Subscribe to it
        let eventBind = sm.RegisterListener(x, pressure, SensorDelay.Normal)                                                  
               
        // find the main label and assign it
        mainLabel := x.FindViewById<TextView>(Resource_Id.mainLabel)
        
    interface ISensorEventListener with
        member x.OnSensorChanged(pressureEvent) =
            Console.WriteLine("Under pressure {0}",pressureEvent.ToString())
            let hPAs = pressureEvent.Values.[0]
            let msg = sprintf "Current pressure: %f hPA!" hPAs
            (!mainLabel).Text <- msg
            let calcedAltitude = calculateAltitudeInFeet(float hPAs)
            x.FindViewById<TextView>(Resource_Id.barometer).Text <- String.Format("Calculated altitude is {0} ft", calcedAltitude)

        member x.OnAccuracyChanged(sensor,  accuracy) =
            Console.WriteLine("Things have changed")




