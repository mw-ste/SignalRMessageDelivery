using Backend.Aggregates;
using Backend.Database;
using Backend.Messaging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly ISenderDatabase _senderDatabase;
    private readonly IPendingMessageDatabase _pendingMessageDatabase;
    private readonly IMediator _mediator;

    public MessageController(
        ISenderDatabase senderDatabase,
        IPendingMessageDatabase pendingMessageDatabase,
        IMediator mediator)
    {
        _senderDatabase = senderDatabase;
        _pendingMessageDatabase = pendingMessageDatabase;
        _mediator = mediator;
    }

    [HttpPost(nameof(Test))]
    public async Task<ActionResult> Test()
    {
        await _senderDatabase.Save(new Sender("peter"));
        await _mediator.Send(new SendMessageCommand("peter", "horst", "test"));
        return Ok();
    }

    [HttpPost(nameof(AddSender))]
    public async Task<ActionResult> AddSender([Required] string senderId)
    {
        await _senderDatabase.Save(new Sender(senderId));
        return Ok();
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
        return Ok(messages.Where(m => m.Retryable).ToArray());
    }

    [HttpGet(nameof(ListFailedMessages))]
    public async Task<ActionResult<PendingMessage[]>> ListFailedMessages()
    {
        var messages = await _pendingMessageDatabase.List();
        return Ok(messages.Where(m => !m.Retryable).ToArray());
    }

    [HttpPost(nameof(RetryFailedMessages))]
    public async Task<ActionResult> RetryFailedMessages()
    {
        var messages = await _pendingMessageDatabase.List();
        foreach (var message in messages)
        {
            message.Revive().Send();
            await _pendingMessageDatabase.Save(message);
        }

        return Ok();
    }
}