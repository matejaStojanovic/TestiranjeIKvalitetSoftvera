using TransactionNow.Domain.Entities;
using TransactionNow.Application.Interfaces;

namespace TransactionNow.Application.Services;

public class CardService
{
    private readonly IUserRepository _userRepository;

    public CardService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task AddCard(string userId, string cardNumber)
{
    if (string.IsNullOrWhiteSpace(userId))
        throw new ArgumentException("User is required.");

    if (string.IsNullOrWhiteSpace(cardNumber))
        throw new ArgumentException("Card number is required.");

    if (cardNumber.Length < 8)
        throw new ArgumentException("Card number is invalid.");

    var user = await _userRepository.GetById(userId);
    if (user == null)
        throw new InvalidOperationException("User not found.");

    if (user.Cards.Any(c => c.CardNumber == cardNumber))
        throw new InvalidOperationException("Card already exists.");

    user.Cards.Add(new Card
    {
        UserId = userId,
        CardNumber = cardNumber,
        AddedAt = DateTime.UtcNow
    });

    await _userRepository.Update(user);
}

    public async Task<User?> GetUserWithCards(string userId)
    {
        return await _userRepository.GetById(userId);
    }

    public async Task RemoveCard(string userId, string cardId)
    {
        var user = await _userRepository.GetById(userId);
        if (user == null)
            throw new Exception("User not found");

        var card = user.Cards.FirstOrDefault(c => c.Id == cardId);
        if (card == null)
            throw new Exception("Card not found");

        user.Cards.Remove(card);

        await _userRepository.Update(user);
    }
}
