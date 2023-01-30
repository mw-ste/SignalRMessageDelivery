using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Backend.Messaging;
using MediatR;

namespace Backend;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly ISenderDatabase _senderDatabase;
    private readonly IMediator _mediator;
    private readonly ILogger<MessageController> _logger;

    public MessageController(
        ISenderDatabase senderDatabase,
        IMediator mediator,
        ILogger<MessageController> logger)
    {
        _senderDatabase = senderDatabase;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost(nameof(SendMessageToClient))]
    public async Task<ActionResult> SendMessageToClient(
        [Required] string sender, 
        [Required] string client, 
        [Required] string message)
    {
        await _mediator.Send(new SendMessageCommand(sender, client, message));
        return Ok();
    }

    [HttpPost(nameof(AddSender))]
    public async Task<ActionResult> AddSender([Required] string senderId)
    {
        await _senderDatabase.Save(new Sender(senderId), _mediator);
        return Ok();
    }

    [HttpGet(nameof(ListSenders))]
    public async Task<ActionResult<Sender[]>> ListSenders()
    {
        var senders = await _senderDatabase.List();
        return Ok(senders);
    }

    [HttpGet(nameof(ListMessagesForSender))]
    public async Task<ActionResult<string[]>> ListMessagesForSender([Required] string senderId)
    {
        var sender = await _senderDatabase.Find(senderId);
        return Ok(sender.MessageLog.ToArray());
    }
}