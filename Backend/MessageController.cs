using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Backend.Messaging;
using Backend.SignalR;
using MediatR;

namespace Backend;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly ISenderDatabase _senderDatabase;
    private readonly IPendingMessageDatabase _pendingMessageDatabase;
    private readonly IMediator _mediator;
    private readonly MessageSchedulerBackgroundService _messageScheduler;

    public MessageController(
        ISenderDatabase senderDatabase,
        IPendingMessageDatabase pendingMessageDatabase,
        IMediator mediator,
        MessageSchedulerBackgroundService messageScheduler)
    {
        _senderDatabase = senderDatabase;
        _pendingMessageDatabase = pendingMessageDatabase;
        _mediator = mediator;
        _messageScheduler = messageScheduler;
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

    [HttpGet(nameof(ListPendingMessages))]
    public async Task<ActionResult<PendingMessage[]>> ListPendingMessages()
    {
        var messages = await _pendingMessageDatabase.List();
        return Ok(messages.ToArray());
    }

    [HttpGet(nameof(ListFailedMessages))]
    public ActionResult<PendingMessage[]> ListFailedMessages()
    {
        var messages = _messageScheduler.DeadMessages;
        return Ok(messages.ToArray());
    }

    [HttpPost(nameof(RetryFailedMessages))]
    public async Task<ActionResult> RetryFailedMessages()
    {
        await _messageScheduler.ReviveDeadMessages();
        return Ok();
    }
}