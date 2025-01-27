## Usage

The Gdl90Manager class is designed to decode GDL90 frames from a streaming data source (such as a UDP client) and emit whole GDL90 messages.

To use this package, instantiate the Gdl90Manager class, subscribe to its NewMessage event, and start passing GDL90 frames to it via the Process method.

```csharp
class Main {
    public static void HandleNewMessage(object o, byte[] gdl90Message) 
    {
        // do something with the message
        Console.WriteLine("[{0}]", string.Join(", ", gdl90Message));
    }
    
    public void Main() 
    {
        var gdl90 = new Gdl90Manager();
        gdl90.NewMessage += HandleNewMessage;
        
        var heartbeat = new byte[]{0x7E, 0x00, 0x81, 0x41, 0xDB, 0xD0, 0x08, 0x02, 0xB3, 0x8B, 0x7E};
        gdl90.Process(heartbeat);
    }
}
```

## To-Do

The Gdl90Manager only decodes GDL90 frames and emits whole messages as a byte array. The next step is to create decoders and structs for each of the standard message types that can decode these GDL90 messages.
