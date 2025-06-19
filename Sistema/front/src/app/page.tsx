'use client';

import { useEffect } from 'react';
import Script from 'next/script';

export default function Home() {
  useEffect(() => {
    if (typeof window !== 'undefined') {
      // Inicialização do gráfico e dados dos alunos
      const students = [
        { id: 1, name: "Ana Clara Silva", status: "present" },
        { id: 2, name: "Bruno Oliveira Santos", status: "present" },
        { id: 3, name: "Carlos Eduardo Pereira", status: "absent" },
        // ... outros alunos ...
      ];

      // Renderizar lista de presença
      const attendanceList = document.getElementById('attendanceList');
      if (attendanceList) {
        students.forEach((student, index) => {
          const row = document.createElement('tr');
          let statusClass = '';
          let statusText = '';
          
          switch(student.status) {
            case 'present':
              statusClass = 'attendance-present';
              statusText = 'Presente';
              break;
            case 'absent':
              statusClass = 'attendance-absent';
              statusText = 'Falta';
              break;
            case 'late':
              statusClass = 'attendance-late';
              statusText = 'Atraso';
              break;
          }
          
          row.innerHTML = `
            <td class="px-6 py-4 whitespace-nowrap">${index + 1}</td>
            <td class="px-6 py-4 whitespace-nowrap">${student.name}</td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="px-2 py-1 rounded-full text-xs font-medium ${statusClass}" id="status-${student.id}">
                ${statusText}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <button onclick="changeStatus(${student.id}, 'present')" class="px-2 py-1 text-xs rounded mr-1 bg-green-100 text-green-800 hover:bg-green-200">P</button>
              <button onclick="changeStatus(${student.id}, 'absent')" class="px-2 py-1 text-xs rounded mr-1 bg-red-100 text-red-800 hover:bg-red-200">F</button>
              <button onclick="changeStatus(${student.id}, 'late')" class="px-2 py-1 text-xs rounded bg-yellow-100 text-yellow-800 hover:bg-yellow-200">A</button>
            </td>
          `;
          
          attendanceList.appendChild(row);
        });
      }

      // Eventos de clique
      const sidebarToggle = document.getElementById('sidebarToggle');
      const sidebar = document.querySelector('.sidebar');
      const saveAttendanceBtn = document.getElementById('saveAttendance');
      const successModal = document.getElementById('successModal');
      const closeModalBtn = document.getElementById('closeModal');
      
      if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', () => {
          sidebar.classList.toggle('active');
        });
      }
      
      if (saveAttendanceBtn && successModal && closeModalBtn) {
        saveAttendanceBtn.addEventListener('click', () => {
          successModal.classList.remove('hidden');
        });
        
        closeModalBtn.addEventListener('click', () => {
          successModal.classList.add('hidden');
        });
      }
    }
  }, []);

  return (
    <>
      <Script src="https://cdn.tailwindcss.com" />
      <Script src="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
      <Script src="https://cdn.jsdelivr.net/npm/chart.js" />

      <style jsx global>{`
        @import url('https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap');
        
        body {
          font-family: 'Poppins', sans-serif;
        }
        
        .sidebar {
          transition: all 0.3s ease;
        }
        
        @media (max-width: 768px) {
          .sidebar {
            transform: translateX(-100%);
          }
          .sidebar.active {
            transform: translateX(0);
          }
        }
        
        .attendance-present {
          background-color: #d1fae5;
          color: #065f46;
        }
        
        .attendance-absent {
          background-color: #fee2e2;
          color: #991b1b;
        }
        
        .attendance-late {
          background-color: #fef3c7;
          color: #92400e;
        }
      `}</style>

      <div className="bg-gray-100">
        {/* Sidebar Toggle for Mobile */}
        <div className="md:hidden fixed top-4 left-4 z-50">
          <button id="sidebarToggle" className="p-2 rounded-md bg-white shadow-md text-gray-700">
            <i className="fas fa-bars"></i>
          </button>
        </div>

        {/* Sidebar */}
        <div className="sidebar fixed h-screen w-64 bg-blue-800 text-white p-4 shadow-lg z-40">
          <div className="flex items-center mb-8 mt-4">
            <img src="https://img.icons8.com/color/48/000000/school.png" alt="Logo" className="w-10 h-10" />
            <h1 className="text-xl font-bold ml-2">Escola Elite</h1>
          </div>
          
          <div className="mb-6 p-3 bg-blue-700 rounded-lg">
            <div className="flex items-center">
              <img src="https://ui-avatars.com/api/?name=Professor+Exemplo&background=random" alt="User" className="w-10 h-10 rounded-full" />
              <div className="ml-3">
                <p className="font-semibold">Professor Exemplo</p>
                <p className="text-xs text-blue-200">Administrador</p>
              </div>
            </div>
          </div>
          
          <nav>
            <ul className="space-y-2">
              <li>
                <a href="#" className="flex items-center p-3 rounded-lg bg-blue-700">
                  <i className="fas fa-home mr-3"></i>
                  <span>Dashboard</span>
                </a>
              </li>
              <li>
                <a href="#" className="flex items-center p-3 rounded-lg hover:bg-blue-700">
                  <i className="fas fa-user-graduate mr-3"></i>
                  <span>Alunos</span>
                </a>
              </li>
              <li>
                <a href="#" className="flex items-center p-3 rounded-lg hover:bg-blue-700">
                  <i className="fas fa-chalkboard-teacher mr-3"></i>
                  <span>Professores</span>
                </a>
              </li>
              <li>
                <a href="#" className="flex items-center p-3 rounded-lg hover:bg-blue-700">
                  <i className="fas fa-calendar-alt mr-3"></i>
                  <span>Calendário</span>
                </a>
              </li>
              <li>
                <a href="#" className="flex items-center p-3 rounded-lg hover:bg-blue-700">
                  <i className="fas fa-chart-pie mr-3"></i>
                  <span>Relatórios</span>
                </a>
              </li>
              <li>
                <a href="#" className="flex items-center p-3 rounded-lg hover:bg-blue-700">
                  <i className="fas fa-cog mr-3"></i>
                  <span>Configurações</span>
                </a>
              </li>
            </ul>
          </nav>
          
          <div className="absolute bottom-4 left-4 right-4">
            <a href="#" className="flex items-center p-3 rounded-lg hover:bg-blue-700">
              <i className="fas fa-sign-out-alt mr-3"></i>
              <span>Sair</span>
            </a>
          </div>
        </div>

        {/* Main Content */}
        <div className="ml-0 md:ml-64 min-h-screen transition-all duration-300">
          {/* Header */}
          <header className="bg-white shadow-sm p-4">
            <div className="flex justify-between items-center">
              <h2 className="text-xl font-bold text-gray-800">Controle de Assiduidade</h2>
              <div className="flex items-center space-x-4">
                <div className="relative">
                  <i className="fas fa-bell text-gray-600"></i>
                  <span className="absolute -top-1 -right-1 w-3 h-3 bg-red-500 rounded-full"></span>
                </div>
                <div className="relative">
                  <i className="fas fa-envelope text-gray-600"></i>
                  <span className="absolute -top-1 -right-1 w-3 h-3 bg-blue-500 rounded-full"></span>
                </div>
                <div className="flex items-center">
                  <img src="https://ui-avatars.com/api/?name=Professor+Exemplo&background=random" alt="User" className="w-8 h-8 rounded-full" />
                  <span className="ml-2 text-sm font-medium hidden md:block">Professor Exemplo</span>
                </div>
              </div>
            </div>
          </header>

          {/* Content */}
          <main className="p-4">
            {/* Filters */}
            <div className="bg-white rounded-lg shadow-sm p-4 mb-6">
              <div className="flex flex-col md:flex-row md:items-center md:justify-between">
                <div className="mb-4 md:mb-0">
                  <label htmlFor="turma" className="block text-sm font-medium text-gray-700 mb-1">Turma</label>
                  <select id="turma" className="w-full md:w-64 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
                    <option>7º Ano A</option>
                    <option>7º Ano B</option>
                    <option>8º Ano A</option>
                    <option>8º Ano B</option>
                    <option selected>9º Ano A</option>
                  </select>
                </div>
                <div className="mb-4 md:mb-0">
                  <label htmlFor="disciplina" className="block text-sm font-medium text-gray-700 mb-1">Disciplina</label>
                  <select id="disciplina" className="w-full md:w-64 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500">
                    <option>Matemática</option>
                    <option>Português</option>
                    <option>História</option>
                    <option selected>Ciências</option>
                    <option>Geografia</option>
                  </select>
                </div>
                <div>
                  <label htmlFor="data" className="block text-sm font-medium text-gray-700 mb-1">Data</label>
                  <input type="date" id="data" className="w-full md:w-48 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500" value="2023-11-15" />
                </div>
              </div>
            </div>

            {/* Stats Cards */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
              <div className="bg-white rounded-lg shadow-sm p-4">
                <div className="flex items-center">
                  <div className="p-3 rounded-full bg-green-100 text-green-600 mr-4">
                    <i className="fas fa-user-check"></i>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Presentes</p>
                    <p className="text-xl font-bold">24</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-lg shadow-sm p-4">
                <div className="flex items-center">
                  <div className="p-3 rounded-full bg-red-100 text-red-600 mr-4">
                    <i className="fas fa-user-times"></i>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Faltas</p>
                    <p className="text-xl font-bold">3</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-lg shadow-sm p-4">
                <div className="flex items-center">
                  <div className="p-3 rounded-full bg-yellow-100 text-yellow-600 mr-4">
                    <i className="fas fa-user-clock"></i>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Atrasos</p>
                    <p className="text-xl font-bold">2</p>
                  </div>
                </div>
              </div>
              <div className="bg-white rounded-lg shadow-sm p-4">
                <div className="flex items-center">
                  <div className="p-3 rounded-full bg-blue-100 text-blue-600 mr-4">
                    <i className="fas fa-users"></i>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Total Alunos</p>
                    <p className="text-xl font-bold">29</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Attendance Table */}
            <div className="bg-white rounded-lg shadow-sm overflow-hidden mb-6">
              <div className="p-4 border-b flex justify-between items-center">
                <h3 className="font-semibold text-lg">Lista de Chamada</h3>
                <button id="saveAttendance" className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500">
                  <i className="fas fa-save mr-2"></i>Salvar
                </button>
              </div>
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">N.</th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nome do Aluno</th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                      <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Ações</th>
                    </tr>
                  </thead>
                  <tbody id="attendanceList" className="bg-white divide-y divide-gray-200">
                    {/* Dados serão preenchidos via JavaScript */}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Attendance Summary */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="bg-white rounded-lg shadow-sm p-4">
                <h3 className="font-semibold text-lg mb-4">Resumo por Disciplina</h3>
                <div className="h-64">
                  <canvas id="subjectChart"></canvas>
                </div>
              </div>
              <div className="bg-white rounded-lg shadow-sm p-4">
                <h3 className="font-semibold text-lg mb-4">Últimos Registros</h3>
                <div className="space-y-4">
                  <div className="flex items-start">
                    <div className="p-2 bg-green-100 rounded-full mr-3 mt-1">
                      <i className="fas fa-check text-green-600"></i>
                    </div>
                    <div>
                      <p className="font-medium">15/11/2023 - Ciências</p>
                      <p className="text-sm text-gray-600">24 presentes, 3 faltas, 2 atrasos</p>
                    </div>
                  </div>
                  <div className="flex items-start">
                    <div className="p-2 bg-red-100 rounded-full mr-3 mt-1">
                      <i className="fas fa-exclamation text-red-600"></i>
                    </div>
                    <div>
                      <p className="font-medium">14/11/2023 - Matemática</p>
                      <p className="text-sm text-gray-600">19 presentes, 7 faltas, 3 atrasos</p>
                      <p className="text-xs text-red-500 mt-1"><i className="fas fa-info-circle mr-1"></i>Alta taxa de ausência</p>
                    </div>
                  </div>
                  <div className="flex items-start">
                    <div className="p-2 bg-green-100 rounded-full mr-3 mt-1">
                      <i className="fas fa-check text-green-600"></i>
                    </div>
                    <div>
                      <p className="font-medium">13/11/2023 - História</p>
                      <p className="text-sm text-gray-600">27 presentes, 1 falta, 1 atraso</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </main>

          {/* Modal */}
          <div id="successModal" className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 hidden">
            <div className="bg-white rounded-lg p-6 max-w-sm w-full">
              <div className="flex justify-center mb-4">
                <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
                  <i className="fas fa-check text-green-600 text-xl"></i>
                </div>
              </div>
              <h3 className="text-lg font-medium text-center mb-2">Chamada salva!</h3>
              <p className="text-gray-600 text-center mb-4">A lista de presença foi armazenada com sucesso.</p>
              <button id="closeModal" className="w-full py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700">OK</button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}