# Sample message processor using mediator

The message loop would be:

```text
Get bytes from antenna (using udp)
    Convert bytes[] to a lora message using a lora message factory
    (inside mediator) Find the handler to the lora message type and call it
    If response requires send payload back, send it back
```