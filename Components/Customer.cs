namespace OS_PayPlug.Components
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A DAO which provides a way to query customer resources.
    /// </summary>
    public static class Customer
    {
        /// <summary>
        /// Create a customer.
        /// </summary>
        /// <seealso cref="CreateRaw">If you want to create a customer using a string of JSON data.</seealso>
        /// <param name="customer">Data used to create the customer.</param>
        /// <returns>The created customer.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var customerData = new Dictionary<string, dynamic>
        /// {
        ///     {"email", "john.watson@example.net"},
        ///     {"first_name", "John"},
        ///     {"last_name", "Watson"},
        ///     {"address1", "27 Rue Pasteur"},
        ///     {"address2", null},
        ///     {"city", "Paris"},
        ///     {"postcode", "75018"},
        ///     {"country", "France"},
        ///     {"metadata", new Dictionary<string, object>
        ///         {
        ///             {"segment", "mass"}
        ///         }
        ///     }
        /// };
        /// var customer = Customer.Create(customerData);
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Create(Dictionary<string, object> customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer cannot be null");
            }

            var json = JsonConvert.SerializeObject(customer);
            var response = CreateRaw(json);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve a customer.
        /// </summary>
        /// <seealso cref="RetrieveRaw">If you want to retrieve a customer as a string of JSON data.</seealso>
        /// <param name="customerID">ID of the customer to retrieve.</param>
        /// <returns>The customer asked.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var customer = Customer.Retrieve("cus_6ESfofiMiLBjC6");
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Retrieve(string customerID)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            var response = RetrieveRaw(customerID);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Retrieve all customers.
        /// </summary>
        /// <seealso cref="ListRaw">If you want to retrieve customers as a string of JSON data.</seealso>
        /// <param name="page">The page number.</param>
        /// <param name="per_page">number of customer per page.</param>
        /// <returns>A collection of customers.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var customers = Customer.List();
        /// var customer = customers["data"][0];
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> List(uint? page = null, uint? per_page = null)
        {
            var response = ListRaw(page, per_page);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Update a customer.
        /// </summary>
        /// <seealso cref="UpdateRaw">If you want to update a customer with a string of JSON data.</seealso>
        /// <param name="customerID">ID of the customer to update.</param>
        /// <param name="customer">Data used to update the customer as a string of JSON data.</param>
        /// <returns>The updated customer.</returns>
        /// <example>
        /// <code><![CDATA[
        /// var customerData = new Dictionary<string, dynamic>
        /// {
        ///     {"address1", "20 Rue Pasteur"}
        /// };
        /// var customer = Customer.Update("cus_72F7mCcibttCnv", customerData);
        /// ]]></code>
        /// </example>
        public static Dictionary<string, dynamic> Update(string customerID, Dictionary<string, object> customer)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            if (customer == null)
            {
                throw new ArgumentNullException("customer cannot be null");
            }

            var json = JsonConvert.SerializeObject(customer);
            var response = UpdateRaw(customerID, json);
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);
        }

        /// <summary>
        /// Delete a customer request.
        /// </summary>
        /// <param name="customerID">ID of the customer to delete.</param>
        /// <example>
        /// <code><![CDATA[
        /// Customer.Delete("cus_72F7mCcibttCnv");
        /// ]]></code>
        /// </example>
        public static void Delete(string customerID)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "customer_id", customerID }
            };
            var uri = Routes.Uri(Routes.DeleteCustomer, parameters);
            Service.Delete(uri);
        }

        /// <summary>
        /// Create a customer request with a string of JSON data.
        /// </summary>
        /// <seealso cref="Create">If you want to create a customer request using a Dictionary.</seealso>
        /// <param name="customer">Data used to create the customer as a string of JSON data.</param>
        /// <returns>The created customer.</returns>
        public static string CreateRaw(string customer)
        {
            if (string.IsNullOrEmpty(customer))
            {
                throw new ArgumentNullException("customer cannot be null");
            }

            var uri = Routes.Uri(Routes.CreateCustomer);
            return Service.Post(uri, customer);
        }

        /// <summary>
        /// Retrieve a customer as a string of JSON data.
        /// </summary>
        /// <seealso cref="Retrieve">If you want to retrieve a customer as a Dictionary.</seealso>
        /// <param name="customerID">ID of the customer to retrieve.</param>
        /// <returns>The customer asked.</returns>
        public static string RetrieveRaw(string customerID)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "customer_id", customerID }
            };
            var uri = Routes.Uri(Routes.RetrieveCustomer, parameters);
            return Service.Get(uri);
        }

        /// <summary>
        /// Retrieve all customers as a string of JSON data.
        /// </summary>
        /// <seealso cref="List">If you want to retrieve customers as a Dictionary.</seealso>
        /// <param name="page">The page number.</param>
        /// <param name="per_page">number of payment per page.</param>
        /// <returns>A collection of customers.</returns>
        public static string ListRaw(uint? page = null, uint? per_page = null)
        {
            var query = new Dictionary<string, string>();
            if (page != null)
            {
                query.Add("page", page.ToString());
            }

            if (per_page != null)
            {
                query.Add("per_page", per_page.ToString());
            }

            var uri = Routes.Uri(Routes.ListCustomers, null, query);
            return Service.Get(uri);
        }

        /// <summary>
        /// Update a customer with a string of JSON data.
        /// </summary>
        /// <seealso cref="Update">If you want to update a customer with a Dictionary.</seealso>
        /// <param name="customerID">ID of the customer to update.</param>
        /// <param name="customer">Data used to update the customer as a string of JSON data.</param>
        /// <returns>The updated customer.</returns>
        public static string UpdateRaw(string customerID, string customer)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                throw new ArgumentNullException("customerID cannot be null");
            }

            if (string.IsNullOrEmpty(customer))
            {
                throw new ArgumentNullException("customer cannot be null");
            }

            var parameters = new Dictionary<string, string>
            {
                { "customer_id", customerID }
            };
            var uri = Routes.Uri(Routes.UpdateCustomer, parameters);
            return Service.Patch(uri, customer);
        }
    }
}
