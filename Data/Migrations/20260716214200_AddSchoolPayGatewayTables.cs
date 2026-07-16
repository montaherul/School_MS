using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolPayGatewayTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentGatewayAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentGatewayTransactionId = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewayHealthRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentProviderId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ResponseTimeMs = table.Column<int>(type: "int", nullable: false),
                    SuccessRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRequests = table.Column<int>(type: "int", nullable: false),
                    FailedRequests = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LastCheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSuccessAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayHealthRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayHealthRecords_PaymentProviders_PaymentProviderId",
                        column: x => x.PaymentProviderId,
                        principalTable: "PaymentProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewaySettlements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentProviderId = table.Column<int>(type: "int", nullable: false),
                    SettlementReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderSettlementId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SettlementData = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewaySettlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewaySettlements_PaymentProviders_PaymentProviderId",
                        column: x => x.PaymentProviderId,
                        principalTable: "PaymentProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchoolPayGatewayTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentProviderId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    OnlinePaymentRequestId = table.Column<int>(type: "int", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankTransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StatusMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    RequestPayload = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    ResponsePayload = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    CallbackPayload = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolPayGatewayTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolPayGatewayTransactions_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchoolPayGatewayTransactions_PaymentProviders_PaymentProviderId",
                        column: x => x.PaymentProviderId,
                        principalTable: "PaymentProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewayRefunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentGatewayTransactionId = table.Column<int>(type: "int", nullable: false),
                    RefundReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderRefundId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayRefunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayRefunds_SchoolPayGatewayTransactions_PaymentGatewayTransactionId",
                        column: x => x.PaymentGatewayTransactionId,
                        principalTable: "SchoolPayGatewayTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGatewayWebhooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentProviderId = table.Column<int>(type: "int", nullable: false),
                    PaymentGatewayTransactionId = table.Column<int>(type: "int", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderEventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RawPayload = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGatewayWebhooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayWebhooks_PaymentProviders_PaymentProviderId",
                        column: x => x.PaymentProviderId,
                        principalTable: "PaymentProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentGatewayWebhooks_SchoolPayGatewayTransactions_PaymentGatewayTransactionId",
                        column: x => x.PaymentGatewayTransactionId,
                        principalTable: "SchoolPayGatewayTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayAudits_EventType",
                table: "PaymentGatewayAudits",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayAudits_PaymentGatewayTransactionId",
                table: "PaymentGatewayAudits",
                column: "PaymentGatewayTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayAudits_PerformedAt",
                table: "PaymentGatewayAudits",
                column: "PerformedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayHealthRecords_PaymentProviderId",
                table: "PaymentGatewayHealthRecords",
                column: "PaymentProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayHealthRecords_Status",
                table: "PaymentGatewayHealthRecords",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayRefunds_PaymentGatewayTransactionId",
                table: "PaymentGatewayRefunds",
                column: "PaymentGatewayTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayRefunds_RefundReference",
                table: "PaymentGatewayRefunds",
                column: "RefundReference",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayRefunds_Status",
                table: "PaymentGatewayRefunds",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySettlements_PaymentProviderId",
                table: "PaymentGatewaySettlements",
                column: "PaymentProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySettlements_SettlementReference",
                table: "PaymentGatewaySettlements",
                column: "SettlementReference",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewaySettlements_Status",
                table: "PaymentGatewaySettlements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayWebhooks_PaymentGatewayTransactionId",
                table: "PaymentGatewayWebhooks",
                column: "PaymentGatewayTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayWebhooks_PaymentProviderId_Status",
                table: "PaymentGatewayWebhooks",
                columns: new[] { "PaymentProviderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayWebhooks_Status",
                table: "PaymentGatewayWebhooks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGatewayWebhooks_TransactionReference",
                table: "PaymentGatewayWebhooks",
                column: "TransactionReference");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolPayGatewayTransactions_PaymentMethodId",
                table: "SchoolPayGatewayTransactions",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolPayGatewayTransactions_PaymentProviderId_Status",
                table: "SchoolPayGatewayTransactions",
                columns: new[] { "PaymentProviderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolPayGatewayTransactions_ProviderTransactionId",
                table: "SchoolPayGatewayTransactions",
                column: "ProviderTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolPayGatewayTransactions_Status",
                table: "SchoolPayGatewayTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolPayGatewayTransactions_TransactionReference",
                table: "SchoolPayGatewayTransactions",
                column: "TransactionReference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentGatewayAudits");

            migrationBuilder.DropTable(
                name: "PaymentGatewayHealthRecords");

            migrationBuilder.DropTable(
                name: "PaymentGatewayRefunds");

            migrationBuilder.DropTable(
                name: "PaymentGatewaySettlements");

            migrationBuilder.DropTable(
                name: "PaymentGatewayWebhooks");

            migrationBuilder.DropTable(
                name: "SchoolPayGatewayTransactions");
        }
    }
}
