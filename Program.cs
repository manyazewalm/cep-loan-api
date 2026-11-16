using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/customers", async () =>
{
    var result = new List<object>();

    var connectionString =
        Environment.GetEnvironmentVariable("DATABASE_URL");

    await using var conn = new NpgsqlConnection(connectionString);

    await conn.OpenAsync();

    var sql = @"
        SELECT
            c.customer_number,
            c.full_name,
            c.phone_number,
            c.age,
            c.sex,
            c.business_tin,
            l.loan_status,
            l.approved_amount,
            l.disbursed_amount
        FROM public.customers c
        INNER JOIN public.loans l
            ON c.customer_number = l.customer_number
    ";

    await using var cmd = new NpgsqlCommand(sql, conn);

    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        result.Add(new
        {
            customer_id = reader["customer_number"].ToString(),
            customer_number = reader["customer_number"].ToString(),
            full_name = reader["full_name"].ToString(),
            phone_number = reader["phone_number"].ToString(),
            age = reader["age"],
            gender = reader["sex"].ToString(),
            tin_number = reader["business_tin"].ToString(),
            customer_region = "Addis Ababa",
            loan_status = reader["loan_status"].ToString(),
            approved_amount = reader["approved_amount"],
            disbursed_amount = reader["disbursed_amount"]
        });
    }

    return Results.Ok(result);
});

app.Run();