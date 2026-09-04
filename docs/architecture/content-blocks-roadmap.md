# Arquitectura de Bloques de Contenido: Estado, Limitaciones y Hoja de Ruta

## 1. Estado arquitectónico actual

En la implementación actual, `Course` es el punto de entrada de las operaciones de creación de secciones y lecciones. El modelo mantiene una relación de composición entre `Course`, `Section`, `Lesson` y `ContentBlock`:

$$\text{Course} \longrightarrow \text{Section} \longrightarrow \text{Lesson} \longrightarrow \text{ContentBlock}$$

Esto permite describir a `Course` como raíz de agregado en las operaciones que atraviesan esa jerarquía, aunque esa interpretación DDD debería considerarse una decisión de diseño, no una propiedad que EF Core establezca por sí mismo. En persistencia, cada nivel tiene su propia entidad y clave foránea.

`ContentBlock` es una entidad abstracta asociada a `Lesson`. La configuración actual usa herencia TPH para `TextContent` y `VideoContent` mediante el discriminador `ContentType`.

### Comportamientos que sí están respaldados por el código

* Los constructores de dominio son privados o internos y la creación se canaliza mediante factories y métodos de dominio.
* `Course.Publish()` comprueba que exista al menos una sección y que alguna sección contenga una lección. En el estado actual no comprueba que las lecciones tengan bloques de contenido.
* La adición y eliminación de secciones, lecciones y bloques reindexa `Order` en las colecciones que administra cada entidad.
* Las relaciones requeridas tienen claves foráneas y eliminación en cascada configuradas en EF Core, lo que reduce la posibilidad de registros huérfanos a nivel de base de datos. Esto no sustituye todas las validaciones de aplicación.

## 2. Limitaciones y riesgos a validar

Estas observaciones describen riesgos que pueden adquirir relevancia con el crecimiento del catálogo, el tamaño de los contenidos o la concurrencia. No implican que todos se manifiesten ya en producción.

### 2.1. Carga de grafos amplios

`CourseRepository.GetWithDetailsAsync()` carga cursos con categorías, reseñas, secciones, lecciones y bloques de contenido. Además, las operaciones de publicación, creación de secciones y creación de lecciones reutilizan esa consulta. `AsSplitQuery()` reduce el riesgo de una explosión cartesiana, pero no evita que se materialice y se rastree un grafo amplio cuando la operación solo necesita una parte de él.

El impacto concreto depende del tamaño del curso y de la operación. Para editar un bloque existente, la alternativa preferible sería consultar la lección o el bloque directamente y evitar hidratar el curso completo.

### 2.2. Concurrencia y límites de agregado

No se observa actualmente una propiedad `RowVersion` o un token de concurrencia explícito en `Course`. Por tanto, no es correcto afirmar que cualquier edición de una lección y del precio del curso vaya a producir necesariamente `DbUpdateConcurrencyException`. El riesgo actual es más acotado: varias operaciones pueden cargar y guardar un grafo relacionado amplio, y la estrategia de actualización puede favorecer conflictos o sobrescrituras según el flujo y la configuración de EF Core.

Separar las operaciones de contenido de las modificaciones de metadatos del curso reduciría el alcance de cada unidad de trabajo. Aun así, no eliminaría por sí sola todos los problemas de concurrencia; seguirían siendo necesarias reglas de actualización, tokens de concurrencia o resolución de conflictos cuando corresponda.

### 2.3. Profundidad de las rutas REST

La ruta actual de `LessonsController` es:

```http
/api/courses/{courseId}/sections/{sectionId}/lessons
```

Todavía no existe un controlador específico para operaciones de bloques de contenido. Si esas operaciones se añadieran siguiendo la jerarquía actual, podrían producir rutas con varios identificadores y acoplar al cliente con la estructura completa del curso. El coste es principalmente de ergonomía, autorización y mantenimiento de contratos; la profundidad por sí sola no determina la corrección de la API.

## 3. Hoja de ruta de evolución

| Fase | Objetivo | Estado |
| --- | --- | --- |
| 1. Línea base | Consolidar el modelo actual y medir sus consultas | Vigente |
| 2. Rutas superficiales | Exponer operaciones de contenido con menos contexto anidado | Pendiente |
| 3. Agregados más pequeños | Evaluar `Lesson` como raíz independiente | Pendiente |
| 4. Tipos multimedia | Incorporar quizzes, almacenamiento externo y procesamiento asíncrono | Pendiente |

### Fase 1: Línea base y estabilidad

- [x] Mantener la creación controlada de `Course`, `Section` y `Lesson` mediante factories y métodos de dominio.
- [x] Mantener el polimorfismo TPH para `TextContent` y `VideoContent`.
- [x] Mantener la reindexación de `Order` en las operaciones soportadas.
- [ ] Añadir pruebas que documenten si publicar un curso requiere contenido en las lecciones, si esa es la regla de negocio deseada.
- [ ] Medir el tamaño y el tiempo de `GetWithDetailsAsync()` con cursos representativos antes de fijar un límite de rendimiento.

### Fase 2: Rutas superficiales (*shallow routing*)

- [ ] Evaluar rutas como:
  * `POST /api/lessons/{lessonId}/content-blocks/text`
  * `POST /api/lessons/{lessonId}/content-blocks/video`
  * `PUT /api/content-blocks/{id}`
  * `DELETE /api/content-blocks/{id}`
- [ ] Implementar la autorización mediante una consulta proyectada o un `AnyAsync` que compruebe la pertenencia de la lección al curso del instructor, sin cargar el grafo completo.
- [ ] Verificar con un plan de ejecución que los índices existentes son suficientes. La existencia de una sola consulta no garantiza por sí misma un acceso indexado.
- [ ] Definir cómo se informarán los casos en que el bloque existe pero no pertenece a la lección o al curso indicado.

### Fase 3: Evaluación de agregados más pequeños

- [ ] Analizar si `Lesson` debe convertirse en raíz de agregado, conservando referencias por ID a `Section` y, si es necesario, a `Course`.
- [ ] Determinar qué responsabilidad conservaría `Course` sobre el orden y la publicación, y qué consistencia se aceptaría entre esos datos y los de `Lesson`.
- [ ] Comparar el coste de modificar una lección de forma independiente con el coste de mantener invariantes que atraviesan curso, sección y lección.
- [ ] Considerar este cambio como una reducción del alcance de concurrencia, no como la eliminación completa de bloqueos o conflictos.

### Fase 4: Expansión multimedia

- [ ] Incorporar `QuizContent` como nuevo subtipo de `ContentBlock` tras definir su modelo, validaciones y estrategia de persistencia.
- [ ] Evaluar Azure Blob Storage, Amazon S3 u otra solución de objetos con URLs de subida y descarga delegadas, evitando transportar archivos grandes por la API cuando el flujo lo permita.
- [ ] Desacoplar la transcodificación mediante eventos y un worker cuando existan requisitos de procesamiento, reintentos, observabilidad y consistencia que lo justifiquen.