<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AboutUs.aspx.cs" Inherits="ASPWebSM.AboutUs" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ASPWebSM - Automate Database Development</title>
    <link href="../tailwind.output.css" rel="stylesheet" />
    <style>
        .gradient-text {
            background: linear-gradient(135deg, #f97316 0%, #fb923c 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }

        .fade-in {
            opacity: 0;
            transform: translateY(40px);
            transition: all 1s ease-out;
        }

            .fade-in.visible {
                opacity: 1;
                transform: translateY(0);
            }
        /* Modal styles */
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.6);
            animation: fadeIn 0.3s ease-in-out;
        }

            .modal.show {
                display: flex;
                align-items: center;
                justify-content: center;
            }

        .modal-content {
            background: #1e293b;
            border-radius: 20px;
            padding: 30px;
            max-width: 500px;
            width: 90%;
            position: relative;
            animation: slideUp 0.4s ease-out;
            border: 2px solid #f97316;
            box-shadow: 0 25px 50px -12px rgba(249, 115, 22, 0.5);
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
            }

            to {
                opacity: 1;
            }
        }

        @keyframes slideUp {
            from {
                transform: translateY(50px);
                opacity: 0;
            }

            to {
                transform: translateY(0);
                opacity: 1;
            }
        }

        .navbar {
            backdrop-filter: blur(10px);
            background: rgba(15, 23, 42, 0.95);
            border-bottom: 1px solid #334155;
        }

        .nav-button {
            transition: all 0.3s ease;
        }

            .nav-button:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(249, 115, 22, 0.4);
            }
    </style>
</head>
<body class="min-h-screen text-white" style="background: #0f172a;">
    <form id="form1" runat="server">

        <!-- Navigation Bar -->
        <nav class="navbar fixed top-0 w-full z-50">
            <div class="max-w-7xl mx-auto px-4 py-4">
                <div class="flex items-center justify-between">
                    <!-- Brand Name Only -->
                    <div class="flex items-center">
                        <span class="text-2xl font-bold gradient-text">ASPWebSM</span>
                    </div>

                    <!-- Navigation Links -->
                    <div class="flex items-center space-x-4">
                        <a href="Login.aspx" class="nav-button px-6 py-2 rounded-lg font-semibold text-white border-2 transition-all"
                            style="border-color: #f97316; color: #f97316;"
                            onmouseover="this.style.background='#f97316'; this.style.color='white';"
                            onmouseout="this.style.background='transparent'; this.style.color='#f97316';">Login
                        </a>
                        <a href="Register.aspx" class="nav-button px-6 py-2 rounded-lg font-semibold text-white transition-all"
                            style="background: linear-gradient(135deg, #f97316 0%, #fb923c 100%); box-shadow: 0 4px 14px 0 rgba(249, 115, 22, 0.39);">Register
                        </a>
                    </div>
                </div>
            </div>
        </nav>

        <!-- Hero Section -->
        <section class="relative overflow-hidden pt-32 pb-20 px-4">
            <div class="absolute inset-0 opacity-10">
                <div class="absolute inset-0" style="background: linear-gradient(135deg, #f97316 0%, #0f172a 100%);"></div>
            </div>

            <!-- Animated background elements -->
            <div class="absolute inset-0 overflow-hidden">
                <!--<div class="absolute -top-40 -right-40 w-80 h-200 rounded-full opacity-20"
                    style="background: radial-gradient(circle, #f97316 0%, transparent 70%);">
                </div>
                <div class="absolute -bottom-40 -left-40 w-80 h-80 rounded-full opacity-20"
                    style="background: radial-gradient(circle, #fb923c 0%, transparent 70%);">
                </div>-->
            </div>

            <div class="max-w-6xl mx-auto relative z-10 text-center">
                <div class="mb-8">
                    <svg class="w-24 h-24 mx-auto mb-6" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 7v10c0 2.21 3.582 4 8 4s8-1.79 8-4V7M4 7c0 2.21 3.582 4 8 4s8-1.79 8-4M4 7c0-2.21 3.582-4 8-4s8 1.79 8 4m0 5c0 2.21-3.582 4-8 4s-8-1.79-8-4"></path>
                    </svg>
                </div>

                <h1 class="text-6xl md:text-8xl font-bold mb-6 gradient-text">ASPWebSM
                </h1>

                <p class="text-2xl md:text-3xl text-slate-300 mb-4 font-semibold">
                    ASP.NET Web Stored Procedure Manager
                </p>

                <p class="text-xl md:text-2xl text-slate-400 max-w-3xl mx-auto mb-10">
                    Automate Database Development with Smart Code Generation
                </p>

                <div class="flex flex-col sm:flex-row gap-4 justify-center">
                    <a href="Register.aspx" class="px-8 py-4 rounded-lg font-bold text-lg text-white transition-all transform hover:scale-105"
                        style="background: linear-gradient(135deg, #f97316 0%, #fb923c 100%); box-shadow: 0 10px 30px -10px rgba(249, 115, 22, 0.5);">Get Started Free
                    </a>
                    <a href="#section-project" class="px-8 py-4 rounded-lg font-bold text-lg border-2 transition-all transform hover:scale-105"
                        style="border-color: #f97316; color: #f97316;"
                        onmouseover="this.style.background='rgba(249, 115, 22, 0.1)';"
                        onmouseout="this.style.background='transparent';">Learn More
                    </a>
                </div>

                <div class="mt-12 flex items-center justify-center space-x-8 text-slate-400">
                    <div class="text-center">
                        <p class="text-3xl font-bold text-white">100%</p>
                        <p class="text-sm">Free</p>
                    </div>
                    <div class="h-12 w-px bg-slate-600"></div>
                    <div class="text-center">
                        <p class="text-3xl font-bold text-white">Fast</p>
                        <p class="text-sm">Code Generation</p>
                    </div>
                    <div class="h-12 w-px bg-slate-600"></div>
                    <div class="text-center">
                        <p class="text-3xl font-bold text-white">Secure</p>
                        <p class="text-sm">Enterprise Ready</p>
                    </div>
                </div>
            </div>
        </section>

        <!-- Project Info Section -->
        <section id="section-project" class="py-20 px-4 fade-in">
            <div class="max-w-6xl mx-auto">
                <div class="backdrop-blur-lg rounded-2xl p-8 md:p-12 border transition-all duration-300 hover:shadow-2xl hover:border-orange-500"
                    style="background: #1e293b; border-color: #334155;">
                    <div class="flex items-center mb-6">
                        <svg class="w-10 h-10 mr-4" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 7v10c0 2.21 3.582 4 8 4s8-1.79 8-4V7M4 7c0 2.21 3.582 4 8 4s8-1.79 8-4M4 7c0-2.21 3.582-4 8-4s8 1.79 8 4m0 5c0 2.21-3.582 4-8 4s-8-1.79-8-4"></path>
                        </svg>
                        <h2 class="text-4xl font-bold">The Project</h2>
                    </div>
                    <div class="space-y-6 text-slate-300 text-lg leading-relaxed">
                        <p>
                            <strong class="text-white">ASPWebSM</strong> (ASP.NET Web Stored Procedure Manager) is a powerful web application designed to streamline database development workflows. Our platform enables developers to upload database table schema files and automatically generate optimized stored procedures and method implementations, significantly reducing development time and minimizing human errors.
                        </p>
                        <p>
                            <strong style="color: #f97316;">Why We Built This:</strong> Database development often involves repetitive tasks—writing CRUD operations, stored procedures, and data access methods. We recognized that developers spend countless hours writing similar code patterns for different tables. ASPWebSM automates this process, allowing developers to focus on business logic rather than boilerplate code.
                        </p>
                        <p>
                            <strong style="color: #f97316;">Key Features:</strong> Upload database table schemas in various formats, generate SQL Server stored procedures automatically, create C# data access methods, download generated files instantly for immediate use, secure user authentication and session management, comprehensive activity logging system, support for complex table relationships and constraints, and enterprise-grade code generation following best practices.
                        </p>
                        <div class="grid md:grid-cols-3 gap-6 mt-8">
                            <div class="p-6 rounded-xl border transition-all hover:scale-105 hover:border-orange-500 cursor-pointer"
                                style="background: #0f172a; border-color: #334155;">
                                <svg class="w-8 h-8 mb-3" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4"></path>
                                </svg>
                                <h3 class="text-xl font-bold mb-2">Upload Schema</h3>
                                <p class="text-slate-400">Simply upload your database table structure files</p>
                            </div>
                            <div class="p-6 rounded-xl border transition-all hover:scale-105 hover:border-orange-500 cursor-pointer"
                                style="background: #0f172a; border-color: #334155;">
                                <svg class="w-8 h-8 mb-3" style="color: #fb923c;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4"></path>
                                </svg>
                                <h3 class="text-xl font-bold mb-2">Auto Generate</h3>
                                <p class="text-slate-400">Our engine creates optimized stored procedures and methods</p>
                            </div>
                            <div class="p-6 rounded-xl border transition-all hover:scale-105 hover:border-orange-500 cursor-pointer"
                                style="background: #0f172a; border-color: #334155;">
                                <svg class="w-8 h-8 mb-3" style="color: #fdba74;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"></path>
                                </svg>
                                <h3 class="text-xl font-bold mb-2">Download & Use</h3>
                                <p class="text-slate-400">Get production-ready code files instantly</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <!-- Technologies Section -->
        <section id="section-tech" class="py-20 px-4 fade-in" style="background: rgba(15, 23, 42, 0.5);">
            <div class="max-w-6xl mx-auto">
                <h2 class="text-4xl font-bold mb-12 text-center">Technologies We Use</h2>
                <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">ASP.NET</p>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">C#</p>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">SQL Server</p>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">SQLite</p>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">Tailwind CSS</p>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">JavaScript</p>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">Stored Procedures</p>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 text-center border transition-all duration-300 hover:scale-110 hover:border-orange-500 cursor-pointer"
                        style="background: linear-gradient(135deg, rgba(249, 115, 22, 0.1) 0%, rgba(30, 41, 59, 0.5) 100%); border-color: #334155;">
                        <p class="font-semibold text-lg">Web Forms</p>
                    </div>
                </div>
            </div>
        </section>

        <!-- Contact Section -->
        <section id="section-contact" class="py-20 px-4 fade-in" style="background: rgba(15, 23, 42, 0.5);">
            <div class="max-w-4xl mx-auto">
                <div class="backdrop-blur-lg rounded-2xl p-8 md:p-12 border" style="background: #1e293b; border-color: #334155;">
                    <h2 class="text-4xl font-bold mb-4 text-center">Get In Touch</h2>
                    <p class="text-slate-300 text-center mb-8">
                        Have questions about ASPWebSM? Need support or want to provide feedback? We'd love to hear from you. Send us a message and we'll respond as soon as possible.
                    </p>

                    <asp:Panel ID="pnlStatus" runat="server" Visible="false" CssClass="mb-6 p-4 rounded-lg border">
                        <asp:Label ID="lblStatus" runat="server" CssClass="block"></asp:Label>
                    </asp:Panel>

                    <div class="mb-6 text-center">
                        <p class="text-slate-400">
                            Emails sent this session: <span class="font-bold text-white">
                                <asp:Label ID="lblEmailCount" runat="server" Text="0"></asp:Label>/5</span>
                        </p>
                        <p class="text-sm mt-2" style="color: #fb923c;">
                            ⚠️ Spam protection: Limited to 5 emails per session
                        </p>
                    </div>

                    <div class="space-y-6">
                        <div class="grid md:grid-cols-2 gap-6">
                            <asp:TextBox ID="txtName" runat="server" placeholder="Your Name"
                                CssClass="w-full px-4 py-3 rounded-lg border focus:outline-none transition-colors text-white placeholder-slate-500"
                                Style="background: #0f172a; border-color: #334155;">
                            </asp:TextBox>
                            <asp:TextBox ID="txtEmail" runat="server" placeholder="Your Email" TextMode="Email"
                                CssClass="w-full px-4 py-3 rounded-lg border focus:outline-none transition-colors text-white placeholder-slate-500"
                                Style="background: #0f172a; border-color: #334155;">
                            </asp:TextBox>
                        </div>
                        <asp:TextBox ID="txtSubject" runat="server" placeholder="Subject"
                            CssClass="w-full px-4 py-3 rounded-lg border focus:outline-none transition-colors text-white placeholder-slate-500"
                            Style="background: #0f172a; border-color: #334155;">
                        </asp:TextBox>
                        <asp:TextBox ID="txtMessage" runat="server" placeholder="Your Message" TextMode="MultiLine" Rows="6"
                            CssClass="w-full px-4 py-3 rounded-lg border focus:outline-none transition-colors text-white placeholder-slate-500 resize-none"
                            Style="background: #0f172a; border-color: #334155;">
                        </asp:TextBox>
                        <asp:Button ID="btnSendMessage" runat="server" Text="📧 Send Message" OnClick="btnSendMessage_Click"
                            CssClass="w-full text-white font-bold py-4 rounded-lg transition-all duration-300"
                            Style="background: linear-gradient(135deg, #f97316 0%, #fb923c 100%); box-shadow: 0 4px 14px 0 rgba(249, 115, 22, 0.39); cursor: pointer;" />
                    </div>
                </div>
            </div>
        </section>

        <!-- Team Section -->
        <section id="section-team" class="py-20 px-4 fade-in">
            <div class="max-w-6xl mx-auto">
                <div class="flex items-center justify-center mb-12">
                    <svg class="w-10 h-10 mr-4" style="color: #fb923c;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
                    </svg>
                    <h2 class="text-4xl font-bold">Meet Our Team</h2>
                </div>
                <div class="grid md:grid-cols-3 gap-8 max-w-4xl mx-auto">
                    <div class="backdrop-blur-lg rounded-xl p-6 border transition-all duration-300 hover:scale-105 hover:border-orange-500"
                        style="background: #1e293b; border-color: #334155;">
                        <div class="w-24 h-24 mx-auto mb-4 rounded-full flex items-center justify-center text-2xl font-bold"
                            style="background: linear-gradient(135deg, #f97316 0%, #fb923c 100%);">
                            BG
                        </div>
                        <h3 class="text-xl font-bold text-center mb-2">Brinda Gadoya</h3>
                        <p class="text-center mb-3" style="color: #fb923c;">Diploma in Computer Engineering</p>
                        <div class="flex items-center justify-center text-slate-400">
                            <svg class="w-4 h-4 mr-2" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm0 0c1.306 0 2.417.835 2.83 2M9 14a3.001 3.001 0 00-2.83 2M15 11h3m-3 4h2"></path>
                            </svg>
                            <span class="text-sm font-semibold">ENR: 23020201055</span>
                        </div>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 border transition-all duration-300 hover:scale-105 hover:border-orange-500"
                        style="background: #1e293b; border-color: #334155;">
                        <div class="w-24 h-24 mx-auto mb-4 rounded-full flex items-center justify-center text-2xl font-bold"
                            style="background: linear-gradient(135deg, #f97316 0%, #fb923c 100%);">
                            HD
                        </div>
                        <h3 class="text-xl font-bold text-center mb-2">Hetshi Dekiwadia</h3>
                        <p class="text-center mb-3" style="color: #fb923c;">Diploma in Computer Engineering</p>
                        <div class="flex items-center justify-center text-slate-400">
                            <svg class="w-4 h-4 mr-2" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm0 0c1.306 0 2.417.835 2.83 2M9 14a3.001 3.001 0 00-2.83 2M15 11h3m-3 4h2"></path>
                            </svg>
                            <span class="text-sm font-semibold">ENR: 23020201042 </span>
                        </div>
                    </div>
                    <div class="backdrop-blur-lg rounded-xl p-6 border transition-all duration-300 hover:scale-105 hover:border-orange-500"
                        style="background: #1e293b; border-color: #334155;">
                        <div class="w-24 h-24 mx-auto mb-4 rounded-full flex items-center justify-center text-2xl font-bold"
                            style="background: linear-gradient(135deg, #f97316 0%, #fb923c 100%);">
                            MS
                        </div>
                        <h3 class="text-xl font-bold text-center mb-2">Maharshi Shukla</h3>
                        <p class="text-center mb-3" style="color: #fb923c;">Diploma in Computer Engineering</p>
                        <div class="flex items-center justify-center text-slate-400">
                            <svg class="w-4 h-4 mr-2" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm0 0c1.306 0 2.417.835 2.83 2M9 14a3.001 3.001 0 00-2.83 2M15 11h3m-3 4h2"></path>
                            </svg>
                            <span class="text-sm font-semibold">ENR: 23020201048</span>
                        </div>
                    </div>
                </div>
                <div class="mt-12 text-center">
                    <div class="flex items-center justify-center text-slate-300">
                        <svg class="w-6 h-6 mr-2" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
                        </svg>
                        <p class="text-lg">Project Location: <span class="font-semibold text-white">DARSHAN UNIVERSITY, RAJKOT</span></p>
                    </div>
                </div>
            </div>
        </section>

        <!-- Success Modal -->
        <div id="successModal" class="modal">
            <div class="modal-content">
                <div class="text-center">
                    <div class="w-20 h-20 mx-auto mb-4 rounded-full flex items-center justify-center"
                        style="background: rgba(249, 115, 22, 0.2);">
                        <svg class="w-12 h-12" style="color: #f97316;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                        </svg>
                    </div>
                    <h3 class="text-2xl font-bold mb-2 text-white">Message Sent!</h3>
                    <p class="text-slate-300 mb-6" id="modalMessage">Your message has been sent successfully. We'll get back to you soon!</p>
                    <button onclick="closeModal()" class="px-8 py-3 rounded-lg font-bold text-white transition-all"
                        style="background: linear-gradient(135deg, #f97316 0%, #fb923c 100%);">
                        Got it!
                    </button>
                </div>
            </div>
        </div>

        <!-- Error Modal -->
        <div id="errorModal" class="modal">
            <div class="modal-content">
                <div class="text-center">
                    <div class="w-20 h-20 mx-auto mb-4 rounded-full flex items-center justify-center"
                        style="background: rgba(239, 68, 68, 0.2);">
                        <svg class="w-12 h-12" style="color: #ef4444;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                        </svg>
                    </div>
                    <h3 class="text-2xl font-bold mb-2 text-white">Oops!</h3>
                    <p class="text-slate-300 mb-6" id="errorModalMessage">Something went wrong. Please try again.</p>
                    <button onclick="closeErrorModal()" class="px-8 py-3 rounded-lg font-bold text-white transition-all"
                        style="background: #ef4444;">
                        Close
                    </button>
                </div>
            </div>
        </div>

        <!-- Hidden fields to trigger modal from code-behind -->
        <asp:HiddenField ID="hfShowModal" runat="server" Value="" />
        <asp:HiddenField ID="hfModalType" runat="server" Value="" />
        <asp:HiddenField ID="hfModalMessage" runat="server" Value="" />

        <!-- Footer -->
        <footer class="py-8 px-4 text-center text-slate-400 border-t" style="border-color: #334155;">

            <p class="text-sm text-slate-500">Automating Database Development | ASP.NET + SQL Server + SQLite</p>
        </footer>

    </form>

    <script>
        // Intersection Observer for fade-in animations
        document.addEventListener('DOMContentLoaded', function () {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('visible');
                    }
                });
            }, { threshold: 0.1 });

            document.querySelectorAll('.fade-in').forEach(el => observer.observe(el));

            // Check if modal should be shown
            var showModal = document.getElementById('<%= hfShowModal.ClientID %>').value;
            var modalType = document.getElementById('<%= hfModalType.ClientID %>').value;
            var modalMessage = document.getElementById('<%= hfModalMessage.ClientID %>').value;

            if (showModal === 'true') {
                if (modalType === 'success') {
                    document.getElementById('modalMessage').innerText = modalMessage;
                    document.getElementById('successModal').classList.add('show');
                } else if (modalType === 'error') {
                    document.getElementById('errorModalMessage').innerText = modalMessage;
                    document.getElementById('errorModal').classList.add('show');
                }
                // Reset hidden field
                document.getElementById('<%= hfShowModal.ClientID %>').value = '';
            }
        });

        // Input focus effects
        document.querySelectorAll('input[type="text"], input[type="email"], textarea').forEach(input => {
            input.addEventListener('focus', function () {
                this.style.borderColor = '#f97316';
            });
            input.addEventListener('blur', function () {
                this.style.borderColor = '#334155';
            });
        });

        function closeModal() {
            document.getElementById('successModal').classList.remove('show');
        }

        function closeErrorModal() {
            document.getElementById('errorModal').classList.remove('show');
        }

        // Close modal when clicking outside
        window.onclick = function (event) {
            var successModal = document.getElementById('successModal');
            var errorModal = document.getElementById('errorModal');
            if (event.target == successModal) {
                closeModal();
            }
            if (event.target == errorModal) {
                closeErrorModal();
            }
        }

        // Smooth scroll for anchor links
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    </script>
</body>
</html>
