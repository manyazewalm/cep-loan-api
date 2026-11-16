using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/customers", async () =>
{
    var result = new List<object>();

    var connectionString =
        "Host=ep-delicate-dew-za6vbgqb-pooler.c-2.eu-west-2.aws.neon.tech;" +
        "Database=neondb;" +
        "Username=neondb_owner;" +
        "Password=npg_G9YQR5WuCDOc;" +
        "SSL Mode=Require;" +
        "Trust Server Certificate=true;";

    await using var conn = new NpgsqlConnection(connectionString);

    await conn.OpenAsync();

    var sql = @"
        SELECT
            c.customer_number,
            c.full_name,
            c.phone_number,
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
            customer_number = reader["customer_number"],
            full_name = reader["full_name"],
            phone_number = reader["phone_number"],
            loan_status = reader["loan_status"],
            approved_amount = reader["approved_amount"],
            disbursed_amount = reader["disbursed_amount"]
        });
    }

    return Results.Ok(result);
});

app.Run();