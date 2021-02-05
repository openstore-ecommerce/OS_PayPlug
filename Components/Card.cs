namespace OS_PayPlug.Components
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A DAO which provides a way to query cards resources.
    /// </summary>
    public static class Card
    {
        /// <summary>
        /// Attach a card to a customer.
        /// </summary>
        /// <seealso cref="CreateRaw">If you want to create a card using a string of JSON data.</seealso>
        /// <param name="customerID">ID of the card's owner.</param>
        /// <param name="card">Data used to create the card.</param>
        /// <returns>The created card.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var cardData = new Dictionary<string, dynamic>
        /// {
        ///     {"card", "tok_e34rfkljlkfje"}
        /// };
        /// var card = Card.Create(cardData);
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Create(string customerID, Dictionary<string, dynamic> card)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            if (card == null)
            {
                throw new ArgumentNullException("card cannot be null");
            }

            var json = JsonConvert.SerializeObject(card);
            var response = CreateRaw(customerID, json);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve a customer's card
        /// </summary>
        /// <seealso cref="RetrieveRaw">If you want to retrieve a card as a string of JSON data.</seealso>
        /// <param name="customerID">ID of the card's owner.</param>
        /// <param name="cardToken">Token of the card queried.</param>
        /// <returns>The card asked.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var card = Card.Retrieve("cus_72F7mCcibttCnv", "card_167oJVCpvtR9j8N85LraL2GA");
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Retrieve(string customerID, string cardToken)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            if (string.IsNullOrEmpty(cardToken))
            {
                throw new ArgumentNullException("cardToken cannot be null");
            }

            var response = RetrieveRaw(customerID, cardToken);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve all cards of a customer.
        /// </summary>
        /// <seealso cref="ListRaw">If you want to retrieve cards as a string of JSON data.</seealso>
        /// <param name="customerID">ID of the card's owner.</param>
        /// <param name="page">The page number.</param>
        /// <param name="per_page">number of payment per page.</param>
        /// <returns>A collection of cards.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var cards = Card.List("cus_72F7mCcibttCnv");
        /// var card = cards["data"][0];
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> List(string customerID, uint? page = null, uint? per_page = null)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            var response = ListRaw(customerID, page, per_page);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Delete a customer's card.
        /// </summary>
        /// <param name="customerID">ID of the card's owner.</param>
        /// <param name="cardToken">ID of the card to delete.</param>
        /// <example>
        /// <code><![CDATA[
        ///  Card.Delete("cus_72F7mCcibttCnv", "card_167oJVCpvtR9j8N85LraL2GA");
        /// ]]></code>
        /// </example>
        public static void Delete(string customerID, string cardToken)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            if (string.IsNullOrEmpty(cardToken))
            {
                throw new ArgumentNullException("cardToken cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "customer_id", customerID },
                { "card_token", cardToken }
            };
            var uri = Routes.Uri(Routes.DeleteCard, parameters);
            Service.Delete(uri);
        }

        /// <summary>
        /// Attach a card to a customer as a string of JSON data.
        /// </summary>
        /// <seealso cref="Create">If you want to create a card using a Dictionary.</seealso>
        /// <param name="customerID">ID of the card's owner.</param>
        /// <param name="card">Data used to create the card as a string of JSON data.</param>
        /// <returns>The created card.</returns>
        public static string CreateRaw(string customerID, string card)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            if (string.IsNullOrEmpty(card))
            {
                throw new ArgumentNullException("card cannot be null");
            }

            var uri = Routes.Uri(Routes.CreateCard);
            return Service.Post(uri, card);
        }

        /// <summary>
        /// Retrieve a customer's card as a string of JSON data.
        /// </summary>
        /// <seealso cref="Retrieve">If you want to retrieve a card as a Dictionary.</seealso>
        /// <param name="customerID">ID of the card's owner.</param>
        /// <param name="cardToken">Token of the card queried.</param>
        /// <returns>The card asked.</returns>
        public static string RetrieveRaw(string customerID, string cardToken)
        {
          if (string.IsNullOrEmpty(customerID))
          {
              throw new ArgumentNullException("customerID cannot be null");
          }

          if (string.IsNullOrEmpty(cardToken))
          {
              throw new ArgumentNullException("cardToken cannot be null");
          }

          var parameters = new Dictionary<string, string>
          {
              { "customer_id", customerID },
              { "card_token", cardToken }
          };
          var uri = Routes.Uri(Routes.RetrieveCard, parameters);
          return Service.Get(uri);
        }

        /// <summary>
        /// Retrieve all cards of a customer as a string of JSON data.
        /// </summary>
        /// <seealso cref="List">If you want to retrieve cards as a Dictionary.</seealso>
        /// <param name="customerID">ID of the customer queried.</param>
        /// <param name="page">The page number.</param>
        /// <param name="per_page">number of payment per page.</param>
        /// <returns>A collection of cards.</returns>
        public static string ListRaw(string customerID, uint? page = null, uint? per_page = null)
        {
          if (string.IsNullOrEmpty(customerID))
          {
              throw new ArgumentNullException("customerID cannot be null");
          }

          var parameters = new Dictionary<string, string>
          {
              { "customer_id", customerID },
          };
          var uri = Routes.Uri(Routes.ListCards, parameters);
          return Service.Get(uri);
        }
    }
}
